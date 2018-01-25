<template id="down-popup">
    <transition name="modal">
        <div class="modal-mask">
            <div class="modal-wrapper">
                <div class="modal-container-sm">
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-2">
                                <i class="fa fa-exclamation-circle fa-3x red-text pull-left"  aria-hidden="true"></i>
                            </div>
                            <div class="col-md-10">
                                <h4>Hệ thống đang bị lỗi. Vui lòng báo IT để được hỗ trợ.</h4>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>
<script>
    export default {
        name: 'down-popup',
        template: '#down-popup'
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
        width: 350px;
        height: 150px;
        margin: 0px auto;
        padding: 15px;
        background-color: #fff;
        border-radius: 2px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, .33);
        transition: all .2s ease;
        font-family: Helvetica, Arial, sans-serif;
    }
    .red-text{
       color: red;
    }
    .modal-header h4 {
        margin-top: 5px;
    }
</style>