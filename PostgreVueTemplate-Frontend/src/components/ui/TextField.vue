<script setup lang="ts">
import { PropType, ref } from "vue";

defineProps({
	icon: String,
	appendIcon: String,
	type: String,
	modelValue: String,
	placeholder: String,
	required: Boolean,
	disabled: Boolean,
	height: { type: String, default: "2.5rem" },
	appendButtonType: { type: String as PropType<"button" | "submit" | "reset">, default: "button" },
	round: { type: String, default: "0" },
	autocomplete: String,
	min: [String, Number],
	max: [String, Number],
});

const emit = defineEmits(["update:modelValue", "change", "click:append", "focus", "blur", "keydown"]);

const updateValue = (event: Event) =>
{
	emit("update:modelValue", (event.target as HTMLInputElement).value);
};

const onEndEdit = (event: Event) =>
{
	emit("change", (event.target as HTMLInputElement).value);
};

const onFocus = (event: FocusEvent) =>
{
	emit("focus", event);
};

const onBlur = (event: FocusEvent) =>
{
	emit("blur", event);
};

const onKeydown = (event: KeyboardEvent) =>
{
	emit("keydown", event);
};


const inputRef = ref<HTMLInputElement | null>(null);
function focusInput()
{
	inputRef.value?.focus();
}

</script>

<template>
	<div :style="[`border-radius:${round};`,`height:${height}`]" class="textfield" :class="disabled?'textfield-disabled':''" relative flex items-center gap-1 pa-1 @click="focusInput">
		<Icon :size="height" v-if="icon" :icon="icon"/>
		<slot name="prepend"></slot>
		<input
			ref="inputRef"
			:type="type"
			class="input"
			:value="modelValue"
			@input="updateValue"
			@change="onEndEdit"
			@focus="onFocus"
			@blur="onBlur"
			@keydown="onKeydown"
			:placeholder="placeholder"
			:required="required"
			:disabled="disabled"
			:autocomplete="autocomplete"
			:min="min"
			:max="max"
		>
		<Button :type="appendButtonType" v-if="appendIcon" :round="round" :icon="appendIcon" variant="flat" @click.stop="$emit('click:append', $event)"></Button>
		<slot></slot>
	</div>
</template>

<style scoped>
.input {
	border: none;
	outline: none;
	background-color: transparent;
	width: 100%;
	height: 32px;
}

.textfield-disabled {
	background-color: var(--surface3);
}

input {
	color: var(--onsurface);
}

.textfield {
	border: 1px solid #ccc;
	border-radius: 4px;
	font-size: 1rem;
	width: 100%;
	box-sizing: border-box;
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
