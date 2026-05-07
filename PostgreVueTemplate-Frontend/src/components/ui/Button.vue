<script setup lang="ts">
import { ref, watchEffect, PropType, computed } from "vue";
import { GetTextColor } from "../../utilities/colorUtility";
import { computeFontSize, computeHeight, defaultSize } from "../../utilities/sizeUtility";

const props = defineProps({
	type: { type: String as PropType<"button" | "submit" | "reset">, default: "button" },
	variant: { type: String, default: "default" },
	icon: { type: String, default: null },
	prependIcon: { type: String, default: null },
	color: { type: String, default: null },
	textColor: { type: String, default: null },
	disabled: { type: Boolean, default: false },
	loading: { type: Boolean, default: false },
	size: { type: Number, default: defaultSize },
	height: { type: String, default: null },
	fontSize: { type: String, default: null },
	round: { type: String, default: "0.5rem" },
	padding: { type: String, default: "0.5rem" },
	custom: { type: Boolean, default: false },
	stack: { type: Boolean, default: false },
	flexScroll: { type: Boolean, default: false },
	shadow: { type: String, default: null }
});
const finalTextColor = ref(GetTextColor(props.color));

const cHeight = computed(()=>{ return computeHeight(props.size, props.height); });
const cFontSize = computed(()=>{ return computeFontSize(props.size, props.fontSize); });

watchEffect(()=>
{
	if(props.textColor)
		finalTextColor.value = props.textColor;
	else
		finalTextColor.value = GetTextColor(props.color);
});

</script>

<template>
	<button
		:type="type"
		class="button"
		:disabled="disabled"
		:style="[padding?`padding:${padding};`:'',`border-radius:${round};`,shadow?`box-shadow: ${shadow} rgb(0 0 0 / 10%);`:'',`height:${cHeight};`]"
		:class="[`bg-${color}`, `button-${variant}`, `${disabled?'button-disabled':''}`,`text-${finalTextColor}`,stack?'flex-col items-center':'']"
		inline-block flex items-center >
		<Icon :size="cFontSize" v-if="icon" :color="finalTextColor" :icon="loading?'i-svg-spinners:90-ring':icon" :class="$slots.default?'me-01':''"></Icon>
		<slot v-if="custom" >
		</slot>
		<FlexScroll v-else-if="flexScroll" row>
			<p my-0 w-full :style="`font-size: ${cFontSize};`">
				<slot />
			</p>
		</FlexScroll>
		<p v-else>
			<slot />
		</p>
		<Icon :size="cFontSize" v-if="prependIcon" :color="finalTextColor" :icon="prependIcon" :class="$slots.default?'me-01':''"></Icon>
	</button>
</template>

<style scoped>
@layer component {
	.button {
		border: none;
		cursor: pointer;
		color: var(--onsurface);
		background-color: var(--surface2);
	}

	.button-disabled {
		filter: grayscale;
		opacity: 0.5;
		cursor: auto;
	}

	.button-flat {
		/* background-color: theme("colors.surface"); */
		background-color: transparent;
	}

	.button-flat:hover:enabled {
		background-color: rgb(from var(--onsurface) r g b / 10%);
	}

	.button-flat:active:enabled {
		background-color: rgb(from var(--onsurface) r g b / 20%);
	}

	.button-default:hover:enabled {
		filter: brightness(1.1);
	}

	.button-default:active:enabled {
		filter: brightness(0.9);
	}
}
</style>
