<script setup lang="ts">
import { ref, computed, useId } from "vue";
import { computeFontSize, computeHeight, defaultSize } from "../../utilities/sizeUtility";

const props = defineProps({
	options: { type: Array as () => any[], required: true },
	color: { type: String, default: null },
	placeholder: { type: String, default: "Select an option" },
	label: { type: String, default: "" },
	itemText: { type: String, default: "text" },
	itemValue: { type: String, default: "value" },
	size: { type: Number, default: defaultSize },
	height: { type: String, default: null },
	fontSize: { type: String, default: null },
	cc: { type: String, default: null },
	disabled: { type: Boolean, default: false },
});

const model = defineModel<any>();
const emit = defineEmits(["change"]);

const id = useId();
const isOpen = ref(false);

const cHeight = computed(()=>{ return computeHeight(props.size, props.height); });
const cFontSize = computed(()=>{ return computeFontSize(props.size, props.fontSize); });


function getOptionText(option: any)
{
	if (typeof option === "object" && option !== null && props.itemText in option)
	{
		return option[props.itemText];
	}
	return option;
}

function getOptionValue(option: any)
{
	if (typeof option === "object" && option !== null && props.itemValue in option)
	{
		return option[props.itemValue];
	}
	return option;
}

const selectedText = computed(() =>
{
	if (model.value === null || model.value === undefined || !props.options)
	{
		return props.placeholder;
	}
	const selectedOption = props.options.find(option => getOptionValue(option) === model.value);

	if (selectedOption)
	{
		return getOptionText(selectedOption);
	}

	return props.placeholder;
});

function selectOption(option: any)
{
	const value = getOptionValue(option);
	model.value = value;
	isOpen.value = false;
	emit("change", value);
}

</script>

<template>
	<div relative :style="`height:${cHeight};`">
		<Button :id="id" type="button" :cc="cc" :height="cHeight" :font-size="cFontSize" :color="color" :disabled="disabled" prepend-icon="i-mdi:menu-down" @click="isOpen = !isOpen">
			{{ selectedText }}
		</Button>

		<Menu :element-id="id" v-model:isOpen="isOpen" match-width :attach-listener="false">
			<div role="listbox" :aria-labelledby="label ? `${label}-label` : undefined" max-h-50vh overflow-y-auto rounded>
				<div
					v-for="(option, index) in options"
					:key="index"
					cursor-pointer
					p-2
					hover:bg-primary
					hover:text-oncolor
					:style="`font-size:${cFontSize};`"
					@click="selectOption(option)"
					role="option"
				>
					<slot name="option" :option="option" :index="index">
						{{ getOptionText(option) }}
					</slot>
				</div>
			</div>
		</Menu>
	</div>
</template>
