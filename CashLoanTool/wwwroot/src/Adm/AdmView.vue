<template id="adm-view">
    <div class="container">
        <div class="row">
            <div class="container padding-sm">
                <span v-bind:class="StatusTextClass">
                    {{Status}}
                </span>
            </div>
        </div>
        <div style="overflow: auto">
            <table class="table table-hover">
                <thead>
                    <tr>
                        <td><h5>Username</h5></td>
                        <td><h5>Division</h5></td>
                        <td><h5>Export</h5></td>
                        <td><h5>See all</h5></td>
                        <td><h5>Type</h5></td>
                        <td><h5>Active</h5></td>
                        <td><h5>C.R.U.D</h5></td>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="user in Users">
                        <td class="table-td" nowrap><span class="table-cell-content">{{user.Username}}</span></td>
                        <td class="table-td" nowrap>
                            <span class="table-cell-content">
                                <select class="form-control control-sm"
                                        v-on:change="OnValueChanged(user.Username, 'DivisionName', $event.target.value)">
                                    <option v-for="(d, index) in Divisions"
                                            v-bind:value="d"
                                            v-bind:selected="user.DivisionName==d">
                                        {{d}}
                                    </option>
                                </select>
                            </span>
                        </td>
                        <td>
                            <div class="checkbox">
                                <input type="checkbox" class="no-margin"
                                       v-bind:checked="user.AllowExport"
                                       v-on:click="OnValueChanged(user.Username, 'AllowExport', $event.target.checked)">
                            </div>
                        </td>
                        <td>
                            <div class="checkbox">
                                <input type="checkbox" class="no-margin"
                                       v-bind:checked="user.CanSeeAllRequests"
                                       v-on:click="OnValueChanged(user.Username, 'CanSeeAllRequests', $event.target.checked)">
                            </div>
                        </td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{user.Type}}</span></td>
                        <td class="table-td" nowrap><span class="table-cell-content">{{user.Active? "Yes" : "No"}}</span></td>
                        <td class="table-td" nowrap>
                            <span class="table-cell-content">
                                <button v-on:click="UpdateUser(user.Username)"
                                        v-bind:disabled="!IsChanged(user.Username)"
                                        type="button"
                                        v-bind:class="{'btn btn-sm': !IsChanged(user.Username), 'btn btn-sm btn-primary': IsChanged(user.Username)}">
                                    <i class="fa fa-pencil" aria-hidden="true"></i>
                                </button>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <input name="newUsername" v-model="NewUsername"
                                   v-on:keyup.enter="AddNewUser" type="text"
                                   class="form-control control-sm" />
                        </td>
                        <td>
                            <select class="form-control control-sm" v-model="NewUserDivision">
                                <option v-for="(d, index) in Divisions" v-bind:value="d">{{d}}</option>
                            </select>
                        </td>
                        <td>
                            <div class="checkbox">
                                <label><input type="checkbox" v-model="NewUserAllowExport">Export?</label>
                            </div>
                        </td>
                        <td>
                            <div class="checkbox">
                                <label><input type="checkbox" v-model="NewUserSeeAllRequests">Can see all?</label>
                            </div>
                        </td>
                        <td>
                            <select class="form-control control-sm" disabled="disabled">
                                <option>User</option>
                            </select>
                        </td>
                        <td>
                            <div class="checkbox">
                                <label><input disabled="disabled" type="checkbox" checked="checked">Active?</label>
                            </div>
                        </td>
                        <td>
                            <button v-on:click="AddNewUser"
                                    v-bind:disabled="DisableSubmit"
                                    type="button"
                                    v-bind:class="{'btn': DisableSubmit, 'btn btn-primary': !DisableSubmit}">
                                <span class="glyphicon glyphicon-plus" aria-hidden="true"></span>
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
                          :prev-text="'Prev'"
                          :force-page="OnPage - 1"
                          :next-text="'Next'"
                          :page-class="'page-item'"
                          :page-link-class="'page-link'"
                          :container-class="'pagination pagination-md'">
                </page-nav>
            </div>
        </div>
        <div class="row">
            <div><span class="center-block">Total: {{TotalRows}}</span></div>
        </div>
    </div>
</template>
<script>
    import axios from 'axios'
    import common from '../Common'
    import API from '../Adm/API'
    //import RequestModal from './RequestModal.vue'

    export default {
        name: 'AdmView',
        template: '#adm-view',
        components: {
            'page-nav': require('vuejs-paginate')
        },
        mounted: function () {
            this.Init();
        },
        data: function () {
            return {
                Status: '',
                StatusTextClass: '',
                NewUsername: '',
                NewUserDivision: '',
                NewUserAllowExport: false,
                NewUserSeeAllRequests: false,

                Divisions: [],
                //UsersModel: [],
                Users: [],
                TotalRows: 0,
                TotalPages: 0,
                OnPage: 0,
            };
        },
        computed: {
            GetCurrentRequestListAPI: function () {
                var page = this.$data.OnPage;
                if (page < 1 || page == null) page = 1;
                var api = API.GetUsersURL;
                api = api.replace("{page}", page);
                console.log(api);
                return API.CurrentHost + api;
            },
            DisableSubmit: function () {
                //requires
                if (!this.$data.NewUsername || !this.$data.NewUserDivision)
                    return true;
                return false;
            }
        },
        methods: {
            //init app
            Init: function () {
                var injectedModel = window.model;
                //console.log("Use injected model...");
                //this.$data.UsersModel = injectedModel;
                this.$data.Users = injectedModel.Users;
                this.$data.OnPage = injectedModel.OnPage;
                this.$data.Divisions = injectedModel.Divisions;
                this.UpdatePagination(injectedModel.TotalPages, injectedModel.TotalRows);
            },
            LoadUsers: function (page) {
                this.$data.OnPage = page;
                var url = API.GetUsersURL;
                url = url.replace("{page}", page);
                var that = this;
                axios.get(url)
                    .then(function (response) {
                        //console.log(response);
                        //console.log(response.headers.login);
                        if (response.headers.login) {
                            //Login expired -> Redirect
                            window.location.href = response.headers.login;
                        }
                        //that.$data.UsersModel = response.data;
                        that.$data.Users = response.data.Users;
                        that.UpdatePagination(response.data.TotalPages, response.data.TotalRows);
                    })
                    .catch(function (error) {
                        that.SetStatus('Server exception. Code: ' + error.response.status, 'status-danger');
                    });
            },
            OnValueChanged: function (userName, property, newValue) {
                console.log('Value changed: ' + userName + '-' + property + '-' + newValue);
                //findIndex has limited browser support
                //Not sure if polly fill handle this gracefully
                var index = this.FindUserIndex(userName);
                if (index == -1) {
                    console.log('Cant find ' + userName);
                    that.SetStatus('Error in application. Contact IT for assistant.' + error.response.status, 'status-danger');
                    return;
                }
                //Set new value
                this.$data.Users[index][property] = newValue;
                //Mark as changed
                this.$data.Users[index].Changed = true;

            },
            IsChanged: function (userName) {
                var index = this.FindUserIndex(userName);
                if (index == -1) {
                    console.log('Cant find ' + userName);
                    this.SetStatus('Error in application. Contact IT for assistant.' + error.response.status, 'status-danger');
                    return;
                }
                if (this.$data.Users[index].Changed)
                    return true;
                return false;
            },
            FindUserIndex: function (userName) {
                return this.$data.Users.findIndex(x => x.Username == userName);
            },
            //Update user handler
            UpdateUser: function (userName) {
                var that = this;
                //check string valid
                var url = API.UpdateUserURL;
                var userIndex = that.FindUserIndex(userName);
                axios.post(url, {
                    Username: userName,
                    Division: that.$data.Users[userIndex].DivisionName,
                    ExportRequests: that.$data.Users[userIndex].AllowExport,
                    SeeAllRequests: that.$data.Users[userIndex].CanSeeAllRequests
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
                        that.SetStatus(message, 'status-success');
                        //Remove changed flag in Users
                        that.$data.Users[userIndex].Changed = false;
                        //Makes sure button state gets rendered correctly
                        that.$forceUpdate()
                    }
                    else {
                        that.SetStatus(message, 'status-danger');
                    }
                })
                .catch(function (error) {
                    console.log(error);
                    that.SetStatus("Failed: " + common.errorCodeTranslater(error.response.status), 'status-danger');
                });
            },
            //Add new user handler
            AddNewUser: function () {
                var format = /[ !@#$%^&*()_+\=\[\]{};':"\\|,<>\/?]/;
                var that = this;
                //check string valid
                if (!that.$data.NewUserDivision) {
                    that.SetStatus("Invalid Division!", 'status-danger');
                    return;
                }
                if (!that.$data.NewUsername || format.test(that.$data.NewUsername)) {
                    that.SetStatus("Invalid username!", 'status-danger');
                    return;
                }
                if (that.$data.NewUsername.length > 50) {
                    that.SetStatus("Username must be =< 50 characters!", 'status-danger');
                    return;
                }
                var url = API.AddNewUserURL;
                //console.log(url);
                axios.post(url, {
                    Username: that.$data.NewUsername,
                    Division: that.$data.NewUserDivision,
                    ExportRequests: that.$data.NewUserAllowExport,
                    SeeAllRequests: that.$data.NewUserSeeAllRequests
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
                        that.SetStatus(message, 'status-success');
                        that.RefreshGrid();
                    }
                    else {
                        that.SetStatus(message, 'status-danger');
                    }
                })
                .catch(function (error) {
                    console.log(error);
                    that.SetStatus("Failed: " + common.errorCodeTranslater(error.response.status), 'status-danger');
                });
            },
            RefreshGrid: function () {
                this.LoadUsers(this.$data.OnPage);
            },
            SetStatus: function (status, className) {
                this.$data.Status = status;
                this.$data.StatusTextClass = className;
            },
            ClearStatus: function () {
                this.$data.Status = '';
                this.$data.StatusTextClass = '';
            },
            UpdatePagination: function (totalPages, totalRows) {
                this.$data.TotalPages = totalPages;
                this.$data.TotalRows = totalRows;
            },
            //Page number clicked handler
            PageNavClicked: function (page) {
                this.ClearStatus();
                this.LoadUsers(page);
            }
        }
    };
</script>
<style scoped>
    .control-sm{
        width:inherit;
        display:inherit;
    }
    .no-margin{
        margin: 0px !important;
    }
    .status-danger{
        color: red;
    }
    .status-success{
        color: forestgreen;
    }
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
</style>
