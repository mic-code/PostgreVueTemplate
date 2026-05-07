<script setup lang="ts">
import { computed, ref, watch, nextTick, onMounted } from "vue";

const props = withDefaults(defineProps<{
	modelValue: string;
	placeholder?: string;
	ariaLabel?: string;
	required?: boolean;
	rows?: number;
	autoExpand?: boolean;
	padding?: string;

}>(), {
	variant: "outlined",
	rows: 3,
	modelValue: "",
	placeholder: "",
	ariaLabel: "",
	autoExpand: false,
	padding: "0.5rem",
});

const emit = defineEmits(["update:modelValue"]);
const textareaRef = ref<HTMLTextAreaElement | null>(null);

const value = computed({
	get: () => props.modelValue,
	set: (val) => emit("update:modelValue", val),
});

const adjustHeight = () =>
{
	if (!props.autoExpand || !textareaRef.value) return;
	textareaRef.value.style.height = "auto";
	const style = window.getComputedStyle(textareaRef.value);
	const borderTop = parseFloat(style.borderTopWidth) || 0;
	const borderBottom = parseFloat(style.borderBottomWidth) || 0;
	textareaRef.value.style.height = `${textareaRef.value.scrollHeight + borderTop + borderBottom}px`;
};

watch(() => props.modelValue, () =>
{
	nextTick(adjustHeight);
});

onMounted(() =>
{
	adjustHeight();
});

</script>

<template>
	<textarea
		ref="textareaRef"
		v-model="value"
		:placeholder="placeholder"
		:aria-label="ariaLabel"
		:required="required"
		:rows="rows"
		class="textfield"
		:style="[autoExpand ? 'overflow-y: hidden;' : '',`padding:${padding};`]"
	/>
</template>
<style scoped>
.textfield-disabled {
	background-color: grey;
}

.textfield {
	color: var(--onsurface);
	border: 1px solid #ccc;
	border-radius: 4px;
	font-size: 1rem;
	width: 100%;
	box-sizing: border-box;
	background-color: var(--surface);
	resize: none;
}

.textfield:hover {
	border-color: #888;
	cursor: text;
}

.textfield:focus-within {
	border-color: #007bff;
	outline: none;
	box-shadow: 0 0 0 0.2rem rgb(0 123 255 / 25%);
}
</style>
