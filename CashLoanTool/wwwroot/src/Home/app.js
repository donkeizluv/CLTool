import Vue from 'vue'
import VueRouter from 'vue-router'
import ClView from './CLView.vue'
//innit router
var router = new VueRouter({
    mode: 'history',
    base: 'Home',
    //root: window.location.href,
    routes: [
        { name: 'Default', path: '/', component: ClView },
        { name: 'Index', path: '/Index', component: ClView } //retuns default page = 1
        //{ name: 'Login', path: '/Login', component: Login }
        //{ path: '/:page/:type/:contains', component: ContractsListing }
    ]
});


Vue.use(VueRouter);
new Vue({
    el: '#app',
    router: router,
    render: h => h(ClView),
});