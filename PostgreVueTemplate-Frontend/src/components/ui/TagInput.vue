<script setup lang="ts">
import { ref } from "vue";
import { sleep } from "../../services/api";

defineProps({
	color: { type: String, default: "primary" },
	placeholder: { type: String, default: "tagInputPlaceholder" }
});

const model = defineModel<string[]>({ required: true });
const emit = defineEmits(["change"]);
const chipCon = ref<HTMLElement>();
const chipHeight = ref(80);

const newTag = ref("");

function updateChipHeight()
{
	chipHeight.value = chipCon.value.clientHeight;
}

async function addTag()
{
	const tagToAdd = newTag.value.trim();
	if (tagToAdd && !model.value.includes(tagToAdd))
	{
		model.value.push(tagToAdd);
		emit("change", model.value);
		await sleep(1);
		updateChipHeight();
	}
	newTag.value = "";
}

async function removeTag(index: number)
{
	model.value.splice(index, 1);
	emit("change", model.value);
	await sleep(1);
	updateChipHeight();
}

function removeLastTag()
{
	if (newTag.value === "" && model.value.length > 0)
	{
		model.value.pop();
		emit("change", model.value);
	}
}

function handleKeyDown(event: KeyboardEvent)
{
	if (event.key === " " || event.key === "Enter")
	{
		event.preventDefault();
		addTag();
	}
	else if (event.key === "Backspace")
	{
		removeLastTag();
	}
}

</script>

<template>
	<div class="tag-input" flex flex-col gap-2 >
		<TextField
			v-model="newTag"
			class="tag-input-field"
			:placeholder="$t(placeholder)"
			bg-surface
			text-onsurface
			@keydown="handleKeyDown"
			@blur="addTag"/>
		<div relative :style="`height: ${chipHeight}px;`">
			<div ref="chipCon" absolute min-w-0 flex flex-wrap gap-1 >
				<Chip
					v-for="(tag, index) in model"
					:key="tag"
					:text="tag"
					removable
					:color="color"
					@remove="removeTag(index)"
				/>
			</div>
		</div>
	</div>
</template>
