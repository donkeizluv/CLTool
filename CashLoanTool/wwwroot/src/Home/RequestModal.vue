<template id="request-sending-modal">
    <transition name="modal">
        <div class="modal-mask">
            <div class="modal-wrapper">
                <div class="modal-container-sm">
                    <div>
                        <h4>Additional Services Prompt</h4>
                    </div>
                    <div class="modal-body">
                        <div class="checkbox">
                            <label><input type="checkbox" value="">Service 1</label>
                        </div>
                        <div class="checkbox">
                            <label><input type="checkbox" value="">Service 2</label>
                        </div>
                        <div class="checkbox">
                            <label><input type="checkbox" value="">Service 3</label>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-default">
                            Request
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
        name: 'request-modal',
        template: '#request-sending-modal',
        props: ['contractid'],
        data: function () {
            return {

            };
        },

        methods: {
            SendRequest: function () {

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

    .modal-container {
        width: 650px;
        height: 450px;
        margin: 0px auto;
        padding: 5px 5px;
        background-color: #fff;
        border-radius: 2px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, .33);
        transition: all .2s ease;
        font-family: Helvetica, Arial, sans-serif;
    }

    .modal-container-sm {
        width: 400px;
        height: 250px;
        margin: 0px auto;
        padding: 5px 5px;
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