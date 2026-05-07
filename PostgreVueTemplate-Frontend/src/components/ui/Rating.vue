<template>
	<div class="d-flex align-center">
		<v-icon
			v-for="i in 5"
			:key="i"
			:icon="i <= (hoverRating || modelValue) ? 'mdi-star' : 'mdi-star-outline'"
			:class="[i <= (hoverRating || modelValue)?'star-fill':'']"
			@mouseover="hoverRating = i"
			@mouseleave="hoverRating = 0"
			@click="setRating(i)"
			class="star-icon"
		></v-icon>
	</div>
</template>

<script setup lang="ts">
import { ref } from "vue";

defineProps({
	modelValue: {
		type: Number,
		default: 0,
	},
});

const emit = defineEmits(["update:modelValue"]);

const hoverRating = ref(0);

function setRating(rating: number)
{
	emit("update:modelValue", rating);
}
</script>

<style scoped>
.star-icon {
	cursor: pointer;
}

.star-fill {
	color: yellow;
}
</style>
