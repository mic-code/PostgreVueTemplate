<script setup lang="ts">
defineProps({
	padding: { type: String, default: "1rem" }
});

</script>

<template>
	<div class="card" :style="`padding: ${padding};`">
		<slot />
	</div>
</template>

<style scoped>
.card {
	border: 1px solid var(--surface2);
	border-radius: 0.5rem;
	box-shadow: 0 2px 4px rgb(0 0 0 / 10%);
}
</style>
