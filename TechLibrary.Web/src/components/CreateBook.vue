<template>
    <div v-if="book">
        <b-card tag="article"
                style="max-width: 30rem;"
                class="mb-2 mx-auto">
            <b-row align-v="center">
                <b-col sm="4" class="text-right">
                    <label for="thumbnail-input">Thumbnail URL</label>
                </b-col>
                <b-col sm="8">
                    <b-form-input id="thumbnail-input" class="input" v-model="book.thumbnailUrl" placeholder="Enter a thumbnail URL"></b-form-input>
                </b-col>
            </b-row>
            <b-row align-v="center">
                <b-col sm="4" class="text-right">
                    <label for="isbn-input">ISBN</label>
                </b-col>
                <b-col sm="8">
                    <b-form-input id="isbn-input" class="input" v-model="book.isbn" placeholder="Enter ISBN"></b-form-input>
                </b-col>
            </b-row>
            <b-form-input class="input" v-model="book.title" placeholder="Enter a title"></b-form-input>

            <b-form-textarea class="input" v-model="book.descr" placeholder="Enter a description" max-rows="10"  no-auto-shrink></b-form-textarea>

            <b-button-toolbar justify>
                <b-button to="/" variant="primary">Back</b-button>
                <b-button v-on:click="save" variant="success">Save</b-button>
            </b-button-toolbar>
        </b-card>
    </div>
</template>

<script>
    import axios from 'axios';

    export default {
        name: 'Book',
        props: ["id"],
        data: () => ({
            book: {},
        }),
        methods: {
            async save() {
                const response = await axios.post(`https://localhost:5001/books`, this.book);
                this.$router.push(`/book/${response.data}`);
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .input {
        margin-bottom: 1rem;
    }
</style>