<template id="adm-view">
    <div class="container">
        <div class="row">
            <div class="form-inline padding-sm">
                <label for="newUsername">Username:</label>
                <input name="newUsername" v-model="NewUsername" v-on:keyup.enter="AddNewUser" type="text" class="form-control" />
                <button v-on:click="AddNewUser" type="button" class="btn btn-primary">Add <span class="glyphicon glyphicon-plus" aria-hidden="true"></span></button>
            </div>
        </div>
        <div class="row">
            <div class="container padding-sm">
                <span v-bind:class="StatusTextClass">
                    {{Status}}
                </span>
            </div>
        </div>
        <table class="table table-hover">
            <thead>
                <tr>
                    <td><h5>Username</h5></td>
                    <td><h5>Type</h5></td>
                    <td><h5>Active</h5></td>
                    <td><h5>Description</h5></td>
                </tr>
            </thead>
            <tbody>
                <tr v-for="user in Users">
                    <td class="table-td" nowrap><span class="table-cell-content">{{user.Username}}</span></td>
                    <td class="table-td" nowrap><span class="table-cell-content">{{user.Type}}</span></td>
                    <td class="table-td" nowrap><span class="table-cell-content">{{user.Active? "Yes" : "No"}}</span></td>
                    <td class="table-td" nowrap><span class="table-cell-content">{{user.Description}}</span></td>
                </tr>
            </tbody>
        </table>
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
                UsersModel: [],
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
            }
        },
        methods: {
            //init app
            Init: function () {
                var injectedModel = window.model;
                //console.log("Use injected model...");
                this.$data.UsersModel = injectedModel;
                this.$data.Users = injectedModel.Users;
                this.$data.OnPage = injectedModel.OnPage;
                this.UpdatePagination();
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
                        that.$data.UsersModel = response.data;
                        that.$data.Users = response.data.Users;
                        that.UpdatePagination();
                    })
                    .catch(function (error) {
                        that.$data.Status = 'Server exception. Code: ' + error.response.status;
                        that.$data.StatusTextClass = "status-danger";
                    });
            },
            //Add new user handler
            AddNewUser: function () {
                var format = /[ !@#$%^&*()_+\=\[\]{};':"\\|,<>\/?]/;
                var that = this;
                //check string valid
                
                if (!that.$data.NewUsername || format.test(that.$data.NewUsername)) {
                    that.$data.Status = "Invalid username!";
                    that.$data.StatusTextClass = "status-danger";
                    return;
                }
                if (that.$data.NewUsername.length > 50) {
                    that.$data.Status = "Username must be =< 50 characters!";
                    that.$data.StatusTextClass = "status-danger";
                    return;
                }
                var url = API.AddNewUserURL;
                //console.log(url);
                axios.post(url, {
                    post: that.$data.NewUsername
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
                            that.$data.Status = message;
                            that.$data.StatusTextClass = "status-success";
                            that.RefreshGrid();
                        }
                        else {
                            that.$data.Status = message;
                            that.$data.StatusTextClass = "status-danger";
                        }
                    })
                    .catch(function (error) {
                        console.log(error);
                        that.$data.Status = "Failed: " + common.errorCodeTranslater(error.response.status);
                        that.$data.StatusTextClass = "status-danger";
                    });
            },
            RefreshGrid: function () {
                this.LoadUsers(this.$data.OnPage);
            },
            ClearStatus: function () {
                this.$data.Status = '';
                this.$data.StatusTextClass = '';
            },
            UpdatePagination: function () {
                this.$data.TotalPages = this.$data.UsersModel.TotalPages;
                this.$data.TotalRows = this.$data.UsersModel.TotalRows;
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
