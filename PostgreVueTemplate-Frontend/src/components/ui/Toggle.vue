<script setup lang="ts">
const props = defineProps({

	modelValue: {
		type: Boolean,
		required: true,
	},
	disabled: {
		type: Boolean,
		default: false,
	},
	icon: { type: String, default: null },

	variant: { type: String, default: "default" },
	color: { type: String, default: "primary" },
	loading: { type: Boolean, default: false },
	size: { type: String, default: "20px" }, // height
	round: { type: String, default: "20px" }, // border-radius
	iconColor: { type: String, default: "text-surface" },
	bgColor: { type: String, default: "bg-surface3" }, // border-radius
	shadow: { type: String, default: null },
});

const emit = defineEmits(["update:modelValue"]);

function toggle()
{
	if (!props.disabled && !props.loading)
	{
		emit("update:modelValue", !props.modelValue);
	}
}
</script>

<template>
	<Icon v-if="icon" :icon="icon" :size="size" :color="iconColor"/>
	<div
		class="toggle-switch"
		:class="[
			`toggle-${variant}`,
			{ 'is-disabled': disabled || loading },
			modelValue ? `bg-${color}` : bgColor,
		]"
		:style="{
			width: `calc(${size} * 2)`,
			height: size,
			borderRadius: round,
			boxShadow: shadow ? `${shadow} rgb(0 0 0 / 10%)` : '',
		}"
		@click="toggle"
	>
		<div class="toggle-switch-track"></div>
		<div
			class="toggle-switch-thumb"
			:style="{
				width: `calc(${size} - 4px)`,
				height: `calc(${size} - 4px)`,
				transform: modelValue ? `translateX(calc(${size}))` : 'translateX(0)',
			}"
		>
			<Icon v-if="loading" icon="i-svg-spinners:90-ring" :size="`calc(${size} - 8px)`" color="black"></Icon>
		</div>
	</div>
</template>

<style scoped>
@layer component {
	.toggle-switch {
		position: relative;
		display: inline-block;
		cursor: pointer;
		user-select: none;
		transition: background-color 0.2s;
	}

	.toggle-switch.is-disabled {
		filter: grayscale;
		opacity: 0.5;
		cursor: auto;
	}

	.toggle-default:hover:enabled {
		filter: brightness(1.1);
	}

	.toggle-default:active:enabled {
		filter: brightness(0.9);
	}

	.toggle-switch-track {
		position: absolute;
		width: 100%;
		height: 100%;
		border-radius: inherit;
	}

	.toggle-switch-thumb {
		position: absolute;
		top: 2px;
		left: 2px;
		background-color: white;
		border-radius: 50%;
		transition: transform 0.2s ease-in-out;
		display: flex;
		align-items: center;
		justify-content: center;
	}
}
</style>
