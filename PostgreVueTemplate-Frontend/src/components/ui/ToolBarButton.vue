<script setup lang="ts">
defineProps({
	variant: { type: String, default: "default" },
	isActive: Boolean,
	activeColor: { type: String, default: "primary" },
	inactiveColor: { type: String, default: "secondary" }
});


function HandleClick()
{
}
</script>

<template>
	<Button :size="3" :padding="null" :variant="variant" :color="isActive? activeColor: inactiveColor" round="0" @click="HandleClick" style="height: 64px; width: 64px;" stack custom px-0 py-2 >
		<p my-a text-wrap text-2.5>
			<slot />
		</p>
	</Button>
</template>