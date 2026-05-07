<script setup lang="ts">
import { ref } from "vue";

console.log("setup");

const textFieldValue = ref("");
const showDialog = ref(false);
const sliderValue = ref(50);
const selectValue = ref(null);
const selectOptions = [
	{ label: "Option 1", value: "1" },
	{ label: "Option 2", value: "2" },
	{ label: "Option 3", value: "3" },
];
const toggleValue = ref(false);
const selectedColor = ref("#FF0000");

</script>

<template>
	<div flex flex-col gap-3 pa-3 >
		<div flex gap-3>
			<h1 class="text-2xl font-bold">
				UI Component Samples
			</h1>
			<LightSwitch/>
		</div>

		<Card title="Dialog" bg-surface2>
			<Header icon="i-carbon:popup" >
				Dialog
			</Header>

			<Button color="primary" @click="showDialog = true">
				Show Dialog
			</Button>

			<Dialog v-model="showDialog">
				<Card bg-surface2 text-center style="min-width: 300px;">
					<p>This is the content of the dialog. You can put anything here.</p>
					<div mt-3>
						<Button w-full color="primary" @click="showDialog = false" >{{$t('ok')}}</Button>
					</div>
				</Card>
			</Dialog>
		</Card>


	</div>
</template>
