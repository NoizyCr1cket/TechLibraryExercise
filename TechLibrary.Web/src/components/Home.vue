<template>
    <div class="home">
        <h1>{{ msg }}</h1>

        <b-pagination v-model="currentPage" :total-rows="totalItems" :per-page="perPage" align="center" v-on:change="onPageChanged"></b-pagination>
        <b-table striped hover :items="dataContext" :fields="fields" responsive="sm" :per-page="10" :current-page="currentPage">
            <template v-slot:cell(thumbnailUrl)="data">
                <b-img :src="data.value" thumbnail fluid></b-img>
            </template>
            <template v-slot:cell(title_link)="data">
                <b-link :to="{ name: 'book_view', params: { 'id' : data.item.bookId } }">{{ data.item.title }}</b-link>
            </template>
        </b-table>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Home',
        props: {
            msg: String
        },
        data: () => ({
            fields: [
                { key: 'thumbnailUrl', label: 'Book Image' },
                { key: 'title_link', label: 'Book Title', sortable: true, sortDirection: 'desc' },
                { key: 'isbn', label: 'ISBN', sortable: true, sortDirection: 'desc' },
                { key: 'descr', label: 'Description', sortable: true, sortDirection: 'desc' }

            ],
            items: [],
            currentPage: 1,
            perPage: 10,
            totalItems: 0,
        }),
        
        methods: {
            dataContext(ctx, callback) {
                axios.get(`https://localhost:5001/books?page=${ctx.currentPage}&pageSize=${ctx.perPage}`)
                    .then(response => {
                        this.totalItems = response.data.totalCount;
                        this.currentPage = response.data.pageNumber;
                        callback(response.data.items);
                    });
            },
            onPageChanged(pageNumber) {
                this.currentPage = pageNumber;
            }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

