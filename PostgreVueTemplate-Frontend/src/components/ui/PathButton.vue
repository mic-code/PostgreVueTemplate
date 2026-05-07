<script setup lang="ts">
import { computed, type PropType } from "vue";
import { IsSubPathOf, IsSubPathOfArray } from "../../utilities/routeUtility";
import { useRoute, useRouter } from "vue-router";


const props = defineProps({
	variant: { type: String, default: "default" },
	activeColor: { type: String, default: "primary" },
	inactiveColor: { type: String, default: "" },
	activePaths: Array as PropType<Array<string>>,
	to: String
});

const router = useRouter();
const route = useRoute();
const isActive = computed(() =>
{
	if(props.activePaths)
		return IsSubPathOf(route, props.to) || IsSubPathOfArray(route, props.activePaths);
	else
		return IsSubPathOf(route, props.to);
});

function HandleClick()
{
	router.push(props.to);
}

</script>

<template>
	<Button :variant="variant" :color="isActive? activeColor: inactiveColor" @click="HandleClick">
		<slot />
	</Button>
</template>