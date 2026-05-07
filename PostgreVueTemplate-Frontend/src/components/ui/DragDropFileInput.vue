<script setup lang="ts">
import { ref } from "vue";

const props = defineProps({
	multiple: { type: Boolean, default: true },
	fileCheck: { type: Boolean, default: false },
	acceptedFileTypes: { type: Array<string>, default: null }
});

const emit = defineEmits<{
	change: [ files: File[] | FileList]
}>();

const frame = ref<HTMLElement>();

const isDragOver = ref(false);

async function traverseFileTree(entry: any, files: File[]): Promise<void>
{
	if (entry.isFile)
	{
		await new Promise<void>((resolve, reject) =>
		{
			entry.file((file: File) =>
			{
				files.push(file);
				resolve();
			}, reject);
		});
	}
	else if (entry.isDirectory)
	{
		const dirReader = entry.createReader();
		const entries = await new Promise<any[]>((resolve) =>
		{
			dirReader.readEntries(resolve);
		});
		for (const subEntry of entries)
			await traverseFileTree(subEntry, files);
	}
}

function validateFileTypesThenEmit(files: FileList | File[])
{
	let isValid = true;
	let isProperSize = true;
	// overSizeFiles.value = [];

	for	(let i = 0;i < files.length;i++)
	{
		const file = files[i];
		if(props.fileCheck && props.acceptedFileTypes && file.type && !props.acceptedFileTypes.includes(file.type))
		{
			alert("Not valid File");
			console.error("not valid file " + file.type);
			isValid = false;
		}

		// if(isOverSize(file))
		// {
		// 	overSizeFiles.value.push(file);
		// 	isProperSize = false;
		// }
	}

	if(isValid && isProperSize)
		emit("change", files);
		// emitFiles(files);

	// if(!isValid)
	// 	showInvalidFileTypeDialog.value = true;
	// if(!isProperSize)
	// 	showOversizeDialog.value = true;
}

async function drop(event)
{
	// replaceItem.value = null;
	event.preventDefault();
	isDragOver.value = false;

	const files: File[] = [];
	const items = event.dataTransfer.items;

	if (items && items.length > 0 && items[0].webkitGetAsEntry)
	{
		const promises: Promise<void>[] = [];
		for (let i = 0; i < items.length; i++)
		{
			const entry = items[i].webkitGetAsEntry();
			if (entry)
				promises.push(traverseFileTree(entry, files));
		}
		await Promise.all(promises);
		validateFileTypesThenEmit(files);
	}
	else
	{
		validateFileTypesThenEmit(event.dataTransfer.files);
	}
}

function dragOver(event)
{
	event.preventDefault();
	isDragOver.value = true;
}

function dragleave(event)
{
	event.preventDefault();
	isDragOver.value = false;
}
</script>

<template>
	<div ref="frame" border-dashed @dragover="dragOver" @dragleave="dragleave" @drop="drop" :class="isDragOver?'dragOverlay':''">
		<div h-full w-full flex flex-col items-center justify-center >
			<p>
				<slot/>
			</p>
			<div flex>
				<Chip :size="1.5" color="secondary" ma-3 v-for="fileType in acceptedFileTypes">
					{{ fileType.toUpperCase() }}
				</Chip>
			</div>
			<FileInput :multiple="multiple" :accepted-file-types="fileCheck ? acceptedFileTypes : []" @change="validateFileTypesThenEmit" />
		</div>
	</div>
</template>

<style>
.dragOverlay {
	background-color: rgb(from var(--primary) r g b / 20%);
}

</style>
