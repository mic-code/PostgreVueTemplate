<script setup lang="ts">
import { computed, ref, watchEffect } from "vue";
import { GetTextColor } from "../../utilities/colorUtility";
import { GetThemeColorNames } from "../../themes/themeConfig";

const props = defineProps({
	modelValue: { type: Number,	required: true },
	min: { type: Number, default: 0	},
	max: { type: Number, default: 100 },
	step: {	type: Number, default: 1 },
	label: { type: String, default: "" },
	icon: { type: String, default: null },
	color: { type: String, default: null },
	size: { type: String, default: "1.5rem" },
	loading: { type: Boolean, default: false },
	textColor: { type: String, default: null },
	disabled: { type: Boolean, default: false },
	showValue: { type: Boolean, default: false },
	textShadow: { type: String, default: "none" },
	valueLabel: { type: String, default: null }
});
const finalTextColor = ref(GetTextColor(props.color));

watchEffect(()=>
{
	if(props.textColor)
		finalTextColor.value = props.textColor;
	else
		finalTextColor.value = GetTextColor(props.color);
});

const emit = defineEmits(["update:modelValue"]);

const value = computed({
	get: () => props.modelValue,
	set: (newValue) =>
	{
		emit("update:modelValue", Number(newValue));
	},
});

const uniqueId = `slider-${Math.random().toString(36).substring(2, 9)}`;

const sliderStyle = computed(() =>
{
	const themeColors = GetThemeColorNames();

	let thumbColor = "var(--primary)";
	if (props.color)
	{
		if (themeColors.includes(props.color))
			thumbColor = `var(--${props.color})`;
		else
			thumbColor = props.color;
	}

	return {
		"--slider-thumb-color": thumbColor
	};
});
</script>

<template>
	<div class="slider-container" flex items-center gap-1 >
		<p v-if="label" :for="uniqueId" class="slider-label" :style="`text-shadow:${textShadow};`"
			:class="[`bg-${color}`, `${disabled?'button-disabled':''}`,`text-${finalTextColor}`]" >
			{{ label }}
		</p>
		<Icon :size="size" v-if="icon" :color="finalTextColor" :icon="loading?'i-svg-spinners:90-ring':icon" :class="$slots.default?'me-01':''"></Icon>
		<input
			:id="uniqueId"
			type="range"
			:min="min"
			:max="max"
			:step="step"
			v-model="value"
			class="slider"
			flex-grow-1
			:style="sliderStyle"
		>
		<p v-if="showValue" mx-2 class="slider-value" :style="`text-shadow:${textShadow};`"
			:class="[`bg-${color}`, `${disabled?'button-disabled':''}`,`text-${finalTextColor}`]" >
			<span v-if="valueLabel">
				{{ valueLabel }}
			</span>
			<span v-else>
				{{ modelValue }}
			</span>
		</p>
	</div>
</template>

<style scoped>
.slider-label {
	font-weight: 500;
	white-space: nowrap;
}

.slider {
	appearance: none;
	width: 100%;
	height: 8px;
	border-radius: 5px;
	background: var(--surface);
	opacity: 0.9;
	transition: opacity 0.2s;
}

.slider:hover {
	opacity: 1;
}

.slider::-webkit-slider-thumb {
	appearance: none;
	width: 20px;
	height: 20px;
	border-radius: 50%;
	background: var(--slider-thumb-color);
	cursor: pointer;
	border: 2px solid #fff;
}

.slider::-moz-range-thumb {
	width: 20px;
	height: 20px;
	border-radius: 50%;
	background: var(--slider-thumb-color);
	cursor: pointer;
	border: 2px solid #fff;
}

.slider-value {
	min-width: 3ch;
	text-align: right;
	font-variant-numeric: tabular-nums;
}
</style>
