<template>
    <div v-if="book">
        <b-card :title="editMode ? '' : book.title"
                :img-src="book.thumbnailUrl"
                img-alt="Image"
                img-top
                tag="article"
                style="max-width: 30rem;"
                class="mb-2">
            <b-row v-if="editMode" align-v="center">
                <b-col sm="4" class="text-right">
                    <label for="thumbnail-input">Thumbnail URL</label>
                </b-col>
                <b-col sm="8">
                    <b-form-input id="thumbnail-input" class="input" v-model="thumbnailUrl" placeholder="Enter a thumbnail URL"></b-form-input>
                </b-col>
            </b-row>
            <b-row v-if="editMode" align-v="center">
                <b-col sm="4" class="text-right">
                    <label for="isbn-input">ISBN</label>
                </b-col>
                <b-col sm="8">
                    <b-form-input id="isbn-input" class="input" v-model="book.isbn" placeholder="Enter ISBN"></b-form-input>
                </b-col>
            </b-row>
            <b-form-input class="input" v-if="editMode" v-model="book.title" placeholder="Enter a title"></b-form-input>

            <b-form-textarea class="input" v-if="editMode" v-model="book.descr" placeholder="Enter a description" max-rows="10"  no-auto-shrink></b-form-textarea>
            <b-card-text v-else>
                {{ book.descr }}
            </b-card-text>

            <b-button-toolbar justify>
                <b-button to="/" variant="primary">Back</b-button>
                <b-button v-if="!editMode" v-on:click="editMode = true" variant="outline-primary">Edit</b-button>
                <b-button v-else v-on:click="save" variant="success">Save</b-button>
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
            book: null,
            editMode: false,
            thumbnailUrl: null,
        }),
        mounted() {
            axios.get(`https://localhost:5001/books/${this.id}`)
                .then(response => {
                    this.book = response.data;
                    this.thumbnailUrl = this.book.thumbnailUrl;
                });
        },
        methods: {
            save() {
                this.book.thumbnailUrl = this.thumbnailUrl;
                axios.put(`https://localhost:5001/books/${this.id}`, this.book);
                this.editMode = false;
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