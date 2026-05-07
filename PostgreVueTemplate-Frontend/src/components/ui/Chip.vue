<script setup lang="ts">
import { computed } from "vue";
import { GetTextColor } from "../../utilities/colorUtility";
import { computeFontSize, computeHeight, computeRound, defaultSize } from "../../utilities/sizeUtility";

const props = defineProps({
	variant: { type: String, default: "default" },
	icon: { type: String, default: null },
	color: { type: String, default: null },
	disabled: { type: Boolean, default: false },
	loading: { type: Boolean, default: false },
	size: { type: Number, default: defaultSize },
	height: { type: String, default: null },
	fontSize: { type: String, default: null },
	round: { type: String, default: null },
	text: { type: String, default: null },
	removable: { type: Boolean, default: false }
});

const emit = defineEmits<{
	(e: "remove"): void;
}>();

const textColor = computed(()=>
{
	return GetTextColor(props.color);
});

const cHeight = computed(()=>{ return computeHeight(props.size, props.height); });
const cFontSize = computed(()=>{ return computeFontSize(props.size, props.fontSize); });
const cRound = computed(()=>{ return computeRound(props.size, props.round); });

function onRemove(event: MouseEvent)
{
	event.stopPropagation();
	emit("remove");
}

</script>

<template>
	<div :class="[`bg-${color}`,`text-${textColor}`]" flex items-center justify-center :style="[`border-radius: ${cRound};`,`height:${cHeight};`,`padding: 0 ${size/4}rem`]">
		<Icon v-if="icon" :size="cFontSize" :icon="icon" :color="textColor"/>
		<p :style="`font-size:${cFontSize}; line-height: ${cFontSize};`" m-0 p-0 :class="{ 'ms-1': icon }">
			{{ text }}
			<slot/>
		</p>
		<Button v-if="removable" :font-size="cFontSize" :height="cHeight" text-color="white" padding="0" :size="cFontSize" icon="i-mdi-close" variant="flat" class="remove-button" @click="onRemove">
		</Button>
	</div>
</template>
