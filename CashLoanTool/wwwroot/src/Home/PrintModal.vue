<template id="print-modal">
    <transition name="modal">
        <div class="modal-mask">
            <div class="modal-wrapper">
                <div class="modal-container-sm">
                    <div>
                        <h4>Bổ sung thông tin: </h4>
                    </div>
                    <div class="modal-body">
                        <div>
                            <div class="row">
                                <h4 class="pull-left">Nơi sinh: </h4>
                            </div>
                            <div class="row">
                                <select class="form-control" v-model="POB">
                                    <option v-for="(place, index) in issuers" v-bind:value="index">{{place}}</option>
                                </select>
                            </div>
                            <div class="row">
                                <h4 class="pull-left">Nơi cấp CMND: </h4>
                            </div>
                            <div class="row">
                                <select class="form-control" v-model="Issuer">
                                    <option v-for="(issuer, index) in issuers" v-bind:value="index">{{issuer}}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button v-bind:disabled="DisabledSubmit" class="btn btn-default" v-on:click="OKClicked">
                            OK
                        </button>
                        <button class="btn btn-default" v-on:click="$emit('close')">
                            Cancel
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>
<script>
    //show document modal
    export default {
        name: 'print-modal',
        template: '#print-modal',
        props: ['issuers'],
        data: function () {
            return {
                POB: undefined,
                Issuer: undefined
            };
        },
        computed: {
            DisabledSubmit: function () {
                //Index of 0 case
                if (this.$data.POB != undefined && this.$data.Issuer != undefined)
                    return false;
                return true;
            }
        },
        methods: {
            OKClicked: function () {
                this.$emit('submitprint', { POB: this.$data.POB, Issuer: this.$data.Issuer });
            }
        }
    }
</script>
<style scoped>
    /*vue modal pop up*/
    .feelback-icon {
        padding-left: 0;
        right: 10px;
    }

    .modal-mask {
        position: fixed;
        z-index: 9998;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(0, 0, 0, .7);
        display: table;
        transition: opacity .3s ease;
    }

    .validate-error {
        font-size: smaller;
        font-weight: 700;
        float: left;
    }

    .modal-wrapper {
        display: table-cell;
        vertical-align: middle;
    }

    .modal-container-sm {
        width: 400px;
        height: 310px;
        margin: 0px auto;
        padding: 15px;
        background-color: #fff;
        border-radius: 2px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, .33);
        transition: all .2s ease;
        font-family: Helvetica, Arial, sans-serif;
    }

    .modal-default-button {
        float: right;
    }

    .modal-header h4 {
        margin-top: 5px;
    }
</style>