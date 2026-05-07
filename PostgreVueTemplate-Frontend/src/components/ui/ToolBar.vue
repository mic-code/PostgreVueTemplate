<script setup lang="ts">
import { PropType, ref } from "vue";

const props = defineProps({
	categories: Array as PropType<Array<string>>,
});


const activeCat = ref(props.categories[0]);

function handleCatClick(catValue: string)
{
	activeCat.value = catValue;
}

</script>

<template>
	<div w-full flex flex-col bg-secondary>
		<div flex bg-primary style="height: 1.5rem;">
			<Button :size="1.5" v-for="cat in categories" :color="activeCat==cat? 'secondary': 'primary'" round="0" @click="handleCatClick(cat)">
				{{$t(cat)}}
			</Button>
		</div>
		<div flex>
			<slot :name="activeCat">
			</slot>
		</div>
	</div>
</template>
