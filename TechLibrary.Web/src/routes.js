import Vue from 'vue';
import VueRouter from 'vue-router';

Vue.use(VueRouter);

const Home = () => import(/* webpackChunkName: "Home" */ './components/Home.vue');
const Book = () => import(/* webpackChunkName: "Book" */ './components/Book.vue');
const CreateBook = () => import('./components/CreateBook.vue');

const router = new VueRouter({
  routes: [
    { path: '/', component: Home },
    {
      name: 'book_view',
      path: '/book/:id',
      component: Book,
      props: true
    },
    {
      name: 'create_book_view',
      path: '/book',
      component: CreateBook,
      props: true
    }
  ]
});

export default router;