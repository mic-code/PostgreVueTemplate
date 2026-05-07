<script setup lang="ts">
import { ref } from "vue";

const color = defineModel<string>("color", { default: "#FFFFFF" });
const emit = defineEmits(["editEnded"]);

const props = defineProps<{
	disabled?: boolean;
}>();

// 10 predefined colors
const colorSlots = [
	"#FF5252", // Red
	"#FF9800", // Orange
	"#FFEB3B", // Yellow
	"#4CAF50", // Green
	"#2196F3", // Blue
	"#9C27B0", // Purple
	"#00BCD4", // Cyan
	"#795548", // Brown
	"#607D8B", // Blue Grey
	"#FFFFFF", // White
];

function selectColor(newColor: string)
{
	if (props.disabled) return;
	color.value = newColor;
	emit("editEnded", newColor);
}
</script>

<template>
	<div class="simple-color-picker" :class="{ 'opacity-50 pointer-events-none': disabled }">
		<button
			v-for="slotColor in colorSlots"
			:key="slotColor"
			class="color-slot"
			:class="{ selected: color.toUpperCase() === slotColor.toUpperCase() }"
			:style="{ backgroundColor: slotColor }"
			@click="selectColor(slotColor)"
		></button>
	</div>
</template>

<style scoped>
.simple-color-picker {
	display: grid;
	grid-template-columns: repeat(5, 1fr);
	gap: 0.5rem;
	padding: 0.5rem;
	user-select: none;
}

.color-slot {
	width: 2rem;
	height: 2rem;
	border-radius: 4px;
	border: 2px solid var(--surface-variant, #ccc);
	cursor: pointer;
	transition: transform 0.1s ease, border-color 0.1s ease;
}

.color-slot:hover {
	transform: scale(1.1);
	border-color: var(--primary, #2196F3);
}

.color-slot.selected {
	border: 2px solid var(--primary, #2196F3);
	box-shadow: 0 0 0 2px var(--primary-light, rgba(33, 150, 243, 0.3));
}
</style>
