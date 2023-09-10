<script setup>
import axios from "axios";
</script>

<template>
  <div class="customersView">
    <table>
      <tr v-for="c in customers">
        <td>{{ c.firstname }}</td>
        <td>{{ c.lastname }}</td>
      </tr>
    </table>
  </div>
</template>


<script>
export default {
  data() {
    return {
      customers: []
    }
  },
  async mounted() {
    const data = (await axios.get('customers')).data;
    this.customers = data;
  },
};
</script>
