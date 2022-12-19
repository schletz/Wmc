<script setup>
import axios from "axios";
</script>

<template>
  <div class="newsContent">
    <h3>{{ newsData.headline }}</h3>
    <p>{{ newsData.content }}</p>
    <p>
      Crated at
      {{
        new Date(newsData.created).toLocaleString("de-AT", {
          year: "numeric",
          month: "2-digit",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
        })
      }}
      by {{ newsData.authorFirstname }}
      {{ newsData.authorLastname }}
    </p>
  </div>
</template>

<script>
export default {
  props: {
    newsid: String,
  },
  data() {
    return {
      newsData: {},
    };
  },
  methods: {
    async loadArticle(newsid) {
      try {
        this.newsData = (await axios.get(`news/${newsid}`)).data;
      } catch (e) {
        alert("Fehler beim Laden des Newscontents.");
      }
    },
  },
  async mounted() {
    await this.loadArticle(this.newsid);
  },
  watch: {
    async newsid(newVal) {
      await this.loadArticle(newVal);
    },
  },
};
</script>
