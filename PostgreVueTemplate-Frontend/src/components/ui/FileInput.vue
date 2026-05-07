<script setup lang="ts">
import { computed, ref } from "vue";

const props = defineProps({
	multiple: { type: Boolean, default: true },
	acceptedFileTypes: { type: Array<string>, default: null }
});

defineExpose({
	clear
});

const emit = defineEmits<{
	change: [ files: FileList]
}>();
const fileInput = ref<HTMLInputElement>();
const inputId = `file-input-${Math.random().toString(36).substring(2, 9)}`;

function handleChange()
{
	emit("change", fileInput.value!.files);
}

function clear()
{
	if (fileInput.value)
	{
		fileInput.value.value = "";
	}
}

const acceptedFileTypesString = computed(() =>
{
	let s = "";
	for	(let i = 0;i < props.acceptedFileTypes.length;i++)
	{
		const append = i < props.acceptedFileTypes.length - 1 ? ", " : "";
		s += "." + props.acceptedFileTypes[i] + append;
	}
	return s;
});

</script>

<template>
	<label :for="inputId" class="file-input-label">
		Select File
	</label>
	<input :accept="acceptedFileTypesString" :id="inputId" ref="fileInput" type="file" :multiple="multiple" hidden @change="handleChange">
</template>

<style>
.file-input-label {
	display: inline-block;
	padding: 8px 16px;
	background-color: var(--primary);
	color: white;
	border-radius: 4px;
	cursor: pointer;
	font-weight: 600;
	transition: background-color 0.2s;
}

.file-input-label:hover {
	background-color: var(--secondary);
}
</style>
