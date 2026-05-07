<script setup lang="ts">
defineProps({
	icon: String,
	color: String,
	size: { type: String, default: "2rem" },
});

</script>

<template>
	<div :class="`text-${color}`" flex items-center>
		<div :style="`width: ${size}; height: ${size};`" class="icon" :class="icon"></div>
	</div>
</template>

<style scoped>
.icon {
	width: 100%;
	height: 100%;
}
</style>