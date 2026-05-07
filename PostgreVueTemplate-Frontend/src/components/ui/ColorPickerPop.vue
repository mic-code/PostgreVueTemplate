<script setup lang="ts">
import { ref } from "vue";
import Menu from "./Menu.vue";
import ColorPicker from "./ColorPicker.vue";
import SimpleColorPicker from "./SimpleColorPicker.vue";

const color = defineModel<string>("color", { default: "#FFFFFF" });
const emit = defineEmits(["editEnded"]);

defineProps<{
	simple?: boolean;
	disabled?: boolean;
}>();

const isOpen = ref(false);

// Generate a unique ID for the trigger element
const elementId = `color-picker-trigger-${Math.random().toString(36).substring(2, 9)}`;

function handleEditEnded(newColor: string) {
	emit("editEnded", newColor);
}
</script>

<template>
	<div>
		<div
			:id="elementId"
			class="color-swatch-trigger"
			:class="{ 'opacity-50 cursor-not-allowed': disabled }"
			:style="{ backgroundColor: color }"
		></div>
		<Menu :element-id="elementId" v-model:is-open="isOpen" :disabled="disabled">
			<SimpleColorPicker v-if="simple" v-model:color="color" :disabled="disabled" @click.stop @edit-ended="handleEditEnded"/>
			<ColorPicker v-else v-model:color="color" :disabled="disabled" @click.stop @edit-ended="handleEditEnded"/>
		</Menu>
	</div>
</template>

<style scoped>
.color-swatch-trigger {
	width: 2rem;
	height: 2rem;
	border-radius: 4px;
	border: 1px solid var(--surface-variant, #ccc);
	cursor: pointer;
}
</style>
