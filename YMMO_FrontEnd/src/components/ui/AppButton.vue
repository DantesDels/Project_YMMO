<template>
<router-link v-if="to" :to="to" class="btn-primary">
<slot />
</router-link>

<button v-else class="btn-primary" type="button">
<slot />
</button>
</template>

<script setup>
defineProps({
  to: { type: [String, Object], default: null },
variant: { type: String, default: 'primary' }
});
</script>

<style scoped>
 .btn-primary {
   display: inline-flex;
   align-items: center;
   justify-content: center;
   white-space: nowrap;
   padding: 0.6rem 1.2rem;
   background-color: #1e2956;
   color: white;
   border-radius: 8px;
   font-weight: 600;
   font-size: 18px;
   text-decoration: none;
   transition: all 0.3s ease;
   min-width: fit-content;
   border: none;
   cursor: pointer;
   box-shadow: 0 5px 7px rgba(0,0,0,0.2);
 }

.btn-primary:hover {
  background-color: #3b4a8a;
  transform: translateY(-2px);
  box-shadow: 0 8px 12px rgba(0,0,0,0.8);
}

.btn-primary:active {
  background-color: #161d40;
  transform: translateY(1px) scale(0.98);
  box-shadow: 0 2px 4px rgba(0,0,0,0.3);
  transition: all 0.05s ease;
}
</style>