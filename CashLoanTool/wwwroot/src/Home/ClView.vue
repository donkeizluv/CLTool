<template id="cl-view">
    <div>
        <input-modal v-bind:cities="Cities"
                       v-if="IsShowingInputInfo"
                       v-on:close="IsShowingInputInfo = false"
                    v-on:submit="SubmitRequest">
        </input-modal>
        <div>
            <div class="well padding-sm">
                <div v-bind:class="StatusTextClass">
                    <h4 class="status-bar">{{StatusMessage}}</h4>
                </div>
            </div>
            <div>
                <div class="pull-left"><h5>Nhóm: <b>{{Division}}</b></h5></div>
                <div class="form-inline">
                    <label class="control-label" for="requestField">Số HĐ:</label>
                    <input v-bind:disabled="Loading" name="requestField" 
                           v-model="ContractId" v-on:keyup="DisallowSend" 
                           v-on:keyup.enter="CheckContractClicked" 
                           type="text" class="form-control" />
                    <button v-bind:disabled="Loading" v-on:click="CheckContractClicked" 
                            type="button" class="btn btn-primary">
                        Kiểm tra 
                        <span class="glyphicon glyphicon-check" aria-hidden="true"></span>
                    </button>
                    <button v-bind:disabled="!AllowSend || Loading" 
                            type="button" 
                            v-on:click="ShowInputModal" 
                            v-bind:class="{'btn': !AllowSend || Loading, 'btn btn-success': AllowSend && !Loading}">
                        Gửi <span v-bind:disabled="!AllowSend || Loading" class="glyphicon glyphicon-send" aria-hidden="true">
                            </span>
                    </button>
                </div>
            </div>
        </div>
        <!--scrollable in small width-->
        <div style="overflow: auto">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <td>
                            <div class="center-block">
                                <button v-bind:disabled="Loading" class="btn btn-primary" v-on:click="RefreshGrid">Refresh <i class="fa fa-refresh" aria-hidden="true"></i></button>
                            </div>
                        </td>
                        <td colspan="999">

                            <div class="pull-right">
                                <button class="btn btn-primary" v-bind:disabled="!CanExport" v-on:click="ExportRequests">Export <i class="fa fa-download" aria-hidden="true"></i></button>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td><button class="btn btn-link" v-on:click="OrderByClicked('RequestId')"><span v-html="DisplayOrderButtonStates('RequestId')"></span>Request Số</button></td>
                        <td><h5>Số Tk.</h5></td>
                        <td><h5>Tên</h5></td>
                        <td><h5>CMND</h5></td>
                        <td><h5>SĐT</h5></td>
                        <td><h5>Số HĐ</h5></td>
                        <td><button class="btn btn-link" v-on:click="OrderByClicked('RequestCreateTime')"><span v-html="DisplayOrderButtonStates('RequestCreateTime')"></span>Ngày tạo</button></td>
                        <td><h5>Người tạo</h5></td>
                        <td><h5>In</h5></td>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="request in Requests">
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.RequestId}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.AcctNo}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.IdentityCardName}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.IdentityCard}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.Phone}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.LoanNo}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.RequestCreateTimeString}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{request.Username}}</span></td>
                        <td class="table-td" nowrap>
                            <button v-show="request.HasValidAcctNo" v-on:click="OpenContractPrinting(request.RequestId)" class="btn btn-link">
                                <span class="fa fa-print onepointfive-em"></span>
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="row">
            <div class="center-block">
                <page-nav :page-count="TotalPages"
                          :click-handler="PageNavClicked"
                          :page-range="3"
                          :prev-text="'Trước'"
                          :force-page="OnPage - 1"
                          :next-text="'Sau'"
                          :page-class="'page-item'"
                          :page-link-class="'page-link'"
                          :container-class="'pagination pagination-md'">
                </page-nav>
            </div>
        </div>
        <div class="row">
            <div><span class="center-block">Tổng số: {{TotalRows}}</span></div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios'
    import common from '../Common'
    import API from '../Home/API'
    import InputModal from './InputModal.vue'

    export default {
        name: 'CLView',
        template: '#cl-view',
        components: {
            'page-nav': require('vuejs-paginate'),
            'input-modal': InputModal
        },
        mounted: function () {
            this.Init();
        },
        watch: {
            '$route'(to, from) {
                this.$data.OnPage = to.query.page;
                this.$data.OrderAsc = to.query.asc
                this.$data.OrderBy = to.query.by;
                this.LoadRequests(this.GetCurrentRequestListAPI);
            }
        },
        data: function () {
            return {
                AllowSend: false,
                ContractId: '',
                StatusMessage: '',
                StatusTextClass: '',
                OrderBy: 'RequestId', //default
                OrderAsc: true,
                Loading: false, //prevent clicking while loading new content
                //listing, nav

                IsShowingInputInfo: false,
                PrintingId: '',

                //RequestListingModel: [], //no really need to store this
                Ability: [],
                Division: undefined,
                Cities: [],
                Requests: [],
                TotalRows: 0,
                TotalPages: 0,
                OnPage: 0,
            };
        },
        computed: {
            GetCurrentRequestListAPI: function () {
                var page = this.$data.OnPage;
                if (page < 1 || page == null) page = 1;
                var api = API.GetRequestListingURL;
                api = api.replace("{by}", this.$data.OrderBy);
                api = api.replace("{page}", page);
                api = api.replace("{asc}", this.$data.OrderAsc);
                //console.log(api);
                return API.CurrentHost + api;
            },
            CanExport: function () {
                var i = this.$data.Ability.length;
                while (i--) {
                    if (this.$data.Ability[i] === 'ExportRequests') {
                        return true;
                    }
                }
                return false;
            }
        },
        methods: {
            //init app
            Init: function () {
                //Load injected
                //console.log('component init!');
                var injectedModel = window.model;
                var cities = window.Cities;
                var ability = window.Ability;

                if (!injectedModel || !cities || !ability) {
                    this.$data.StatusMessage = "Error loading app. Contact IT Dept.";
                    this.$data.StatusTextClass = "status-danger";
                    this.$data.Loading = true;
                    return;
                }

                this.$data.OnPage = injectedModel.OnPage;
                this.$data.OrderBy = injectedModel.OrderBy;
                this.$data.OrderAsc = injectedModel.OrderAsc;
                //this.$data.RequestListingModel = injectedModel;
                this.$data.Requests = injectedModel.Requests;
                this.$data.Division = injectedModel.Division;
                this.$data.Cities = cities;
                this.$data.Ability = ability;
                this.UpdatePagination(injectedModel.TotalPages, injectedModel.TotalRows);

            },
            LoadRequests: function (url) {
                this.$data.Loading = true;
                var that = this;
                //that.$data.IsLoading = true; //way too fast to show loading animation, causes jerking in UI
                //console.log(url);
                axios.get(url)
                    .then(function (response) {
                        //console.log(response);
                        //console.log(response.headers.login);
                        if (response.headers.login) {
                            //Login expired -> Redirect
                            window.location.href = response.headers.login;
                        }
                        //that.$data.RequestListingModel = response.data;
                        that.$data.Requests = response.data.Requests;
                        that.UpdatePagination(response.data.TotalPages, response.data.TotalRows);
                        that.$data.Loading = false;
                    })
                    .catch(function (error) {
                        console.log(error);
                        that.$data.StatusMessage = "Load dữ liệu request không thành công. Code: " + error.response.status;
                        that.$data.StatusTextClass = "status-danger";
                        //console.log("Failed to fetch model"); //display this somehow...
                    });
            },

            //update paging
            UpdatePagination: function (totalPages, totalRows) {
                this.$data.TotalPages = totalPages;
                this.$data.TotalRows = totalRows;
            },
            //Page number clicked handler
            PageNavClicked: function (page) {
                ////router.push({ path: `${page}/${type}/${contains}` })
                if (this.$data.Loading) return;
                this.$data.Loading = true;
                this.ClearAllControls();
                this.$data.OnPage = page;
                this.$router.push({ name: 'Index', query: this.ComposeRequestListingAPI(page) });

            },
            
            //anything that causes re-validation of contract id
            DisallowSend: function () {
                this.$data.AllowSend = false;
            },
            ClearStatus: function () {
                this.$data.StatusMessage = '';
                this.$data.StatusTextClass = '';
            },
            SetStatus: function (status, className) {
                this.$data.Status = status;
                this.$data.StatusTextClass = className;
            },
            ClearContractIdField: function () {
                this.$data.ContractId = '';
            },
            ClearAllControls: function () {
                this.DisallowSend();
                this.ClearStatus();
                this.ClearContractIdField();
            },
            //Refresh clicked handler
            RefreshGrid: function () {
                if (this.$data.Loading) return;
                this.ClearAllControls();
                this.$data.Loading = true; //prevent click spamming
                this.LoadRequests(this.GetCurrentRequestListAPI);
            },
            ExportRequests: function () {
                var url = API.CurrentHost + API.ExportRequestURL;
                window.open(url);
            },
            ComposeRequestListingAPI: function (pageNumber) {
                return {
                    page: pageNumber,
                    by: this.$data.OrderBy,
                    asc: this.$data.OrderAsc,
                };
            },
            //order methods
            OrderByClicked: function (orderBy) {
                if (this.$data.Loading) return;
                this.ClearAllControls();
                this.$data.Loading = true; //prevent click spamming
                //if already ordery by this -> change Asc
                if (this.$data.OrderBy == orderBy) {
                    this.$data.OrderAsc = !this.$data.OrderAsc;
                }
                else {
                    //reset Asc
                    this.$data.OrderBy = orderBy;
                    this.$data.OrderAsc = true;
                }
                this.$router.push({ name: 'Index', query: this.ComposeRequestListingAPI(1) });
            },
            //Check contract click handler
            CheckContractClicked: function () {
                var format = /[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/;
                
                var that = this;
                //check string valid
                if (!that.$data.ContractId || format.test(that.$data.ContractId)) {
                    that.$data.StatusMessage = "Số hợp đồng không hợp lệ";
                    that.$data.StatusTextClass = "status-danger";
                    return;
                }
                this.$data.Loading = true;

                var url = API.CheckContractURL;
                //console.log(url);
                axios.post(url, {
                    post: that.$data.ContractId
                })
                    .then(function (response) {
                        if (response.headers.login) {
                            //Login expired -> Redirect
                            window.location.href = response.headers.login;
                        }
                        //console.log(response);
                        var valid = response.data.Valid;
                        var message = response.data.Message;
                        if (valid) {
                            that.$data.StatusMessage = message;
                            that.$data.StatusTextClass = "status-success";
                            that.$data.AllowSend = true;
                        }
                        else {
                            that.$data.StatusMessage = message;
                            that.$data.StatusTextClass = "status-danger";
                            that.$data.AllowSend = false;
                        }
                        that.$data.Loading = false;
                    })
                    .catch(function (error) {
                        console.log(error);
                        that.$data.StatusMessage = "Hợp đồng không hợp lệ: " + common.errorCodeTranslater(error.response.status);
                        that.$data.StatusTextClass = "status-danger";
                        that.$data.AllowSend = false;
                        that.$data.Loading = false;
                    });
            },
            ShowInputModal: function () {
                this.$data.IsShowingInputInfo = true;
            },
            //print document hanlder
            OpenContractPrinting: function (requestId) {
                var s64 = encodeURIComponent(btoa(requestId));
                var url = API.CurrentHost + API.GetDocumentURL;
                url = url.replace('{id}', s64);
                console.log(s64);
                console.log(url);
                window.open(url);
            },
            //Input modal submit handler
            SubmitRequest: function (additionalInfo) {
                this.$data.IsShowingInputInfo = false;
                var that = this;
                var url = API.SendRequestURL;
                //console.log({ ContractId: that.$data.ContractId, IssuePlace: additionalInfo.IssuePlace, Pob: additionalInfo.Pob });
                axios.post(url, {
                    ContractId: that.$data.ContractId,
                    IssuePlace: additionalInfo.IssuePlace,
                    Pob: additionalInfo.Pob
                })
                .then(function (response) {
                    if (response.headers.login) {
                        //Login expired -> Redirect
                        window.location.href = response.headers.login;
                    }
                    //console.log(response);
                    var valid = response.data.Valid;
                    var message = response.data.Message;
                    if (valid) {
                        that.$data.StatusMessage = message;
                        that.$data.StatusTextClass = "status-success";
                        that.$data.AllowSend = false;
                        //Refresh grid, sort no response to top
                        that.LoadRequests(that.GetCurrentRequestListAPI);
                    }
                    else {
                        that.$data.StatusMessage = message;
                        that.$data.StatusTextClass = "status-danger";
                        that.$data.AllowSend = false;
                        that.$data.Loading = false;
                    }
                })
                .catch(function (error) {
                    console.log(error);
                    that.$data.StatusMessage = "Lỗi hệ thông: " + error.response.status + '(' + common.errorCodeTranslater(error.response.status) + ')';
                    that.$data.StatusTextClass = "status-danger";
                    that.$data.AllowSend = false;
                    that.$data.Loading = false;
                });
            },
            DisplayOrderButtonStates: function (orderBy) {
                //console.log(orderBy);
                if (orderBy == this.$data.OrderBy) {
                    if (this.$data.OrderAsc)
                        return "&dtrif;";
                    return "&utrif;";
                }
                return "";
            }
        }
    };
</script>
<style scoped>
    .padding-sm {
        padding: 10px;
    }
    .table-td{
        padding:0px;
        line-height: 3;
    }
    .table-cell-content {
        font-size: small;
        font-weight: 500;
    }
    .status-bar {
        height: 20px;
        margin: 0px;
        padding: 0px;
    }

    .onepointfive-em {
        font-size: 1.5em;
    }

    .status-danger * {
        color: lightcoral;
    }
    .status-success * {
        color: forestgreen;
    }
    /*
 * The following styles are auto-applied to elements with
 * transition="modal" when their visibility is toggled
 * by Vue.js.
 *
 * You can easily play with the modal transition by editing
 * these styles.
 */

    .modal-enter {
        opacity: 0;
    }

    .modal-leave-active {
        opacity: 0;
    }

    .modal-enter .modal-container,
    .modal-leave-active .modal-container {
        -webkit-transform: scale(1.1);
        transform: scale(1.1);
    }
</style>
