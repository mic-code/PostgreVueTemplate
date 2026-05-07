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

const tableHeaders = [
	{ text: "Dessert (100g serving)", value: "name" },
	{ text: "Calories", value: "calories" },
	{ text: "Fat (g)", value: "fat" },
	{ text: "Carbs (g)", value: "carbs" },
	{ text: "Protein (g)", value: "protein" },
];

const tableItems = [
	{ name: "Frozen Yogurt", calories: 159, fat: 6.0, carbs: 24, protein: 4.0 },
	{ name: "Ice cream sandwich", calories: 237, fat: 9.0, carbs: 37, protein: 4.3 },
	{ name: "Eclair", calories: 262, fat: 16.0, carbs: 23, protein: 6.0 },
	{ name: "Cupcake", calories: 305, fat: 3.7, carbs: 67, protein: 4.3 },
	{ name: "Gingerbread", calories: 356, fat: 16.0, carbs: 49, protein: 3.9 },
];
</script>

<template>
	<div flex flex-col gap-3 pa-3 >
		<div flex gap-3>
			<h1 class="text-2xl font-bold">
				UI Component Samples
			</h1>
			<LightSwitch/>
		</div>

		<Card title="Buttons" bg-surface2>
			<Header icon="i-mdi:gesture-tap-button" >
				Buttons
			</Header>
			<div class="flex flex-wrap gap-2">
				<Button>Default</Button>
				<Button color="primary">
					Primary
				</Button>
				<Button color="secondary">
					Secondary
				</Button>
				<Button color="danger">
					danger
				</Button>
			</div>
		</Card>

		<Card title="Menu" bg-surface2>
			<Header icon="i-mdi:menu">
				Menu
			</Header>
			<div class="flex flex-wrap gap-2">
				<Button color="primary" id="testMenu" >Menu</Button>
				<Menu element-id="testMenu" >
					<div flex flex-col>
						<Button round="0" variant="flat" id="testMenu" >Menu Item 1</Button>
						<Button round="0" variant="flat" id="testMenu" >Menu Item 1</Button>
						<Button round="0" variant="flat" id="testMenu" >Menu Item 1</Button>
					</div>
				</Menu>

				<Button color="primary" id="testMenu2" >Menu 2</Button>
				<Menu element-id="testMenu2" >
					<div round="0" flex flex-col gap-3 >
						<Button ma-0 color="primary" id="testMenu" >Menu Item 1</Button>
						<Button color="primary" id="testMenu" >Menu Item 1</Button>
						<Button color="primary" id="testMenu" >Menu Item 1</Button>
					</div>
				</Menu>
			</div>
		</Card>

		<Card title="Alerts" bg-surface2>
			<Header icon="i-clarity:warning-standard-solid" >
				Alert
			</Header>
			<div class="space-y-2">
				<Alert bg-surface>This is a default alert.</Alert>
				<Alert type="primary">
					This is a primary alert.
				</Alert>
				<Alert type="success">
					This is a success alert.
				</Alert>
				<Alert type="warning">
					This is a warning alert.
				</Alert>
				<Alert type="danger">
					This is a danger alert.
				</Alert>
			</div>
		</Card>

		<Card title="Form Inputs" bg-surface2>
			<Header icon="i-radix-icons:input" >
				Textfield
			</Header>
			<div class="space-y-4">
				<TextField
					v-model="textFieldValue"
					label="Name"
					placeholder="Enter your name"
					bg-surface
				/>
				<TextField
					v-model="textFieldValue"
					label="Name"
					placeholder="Disabled"

					disabled bg-surface
				/>
			</div>
		</Card>

		<Card title="Form Inputs" bg-surface2>
			<Header icon="i-mdi:form-select" >
				Select
			</Header>
			<div class="space-y-4">
				<Select
					v-model="selectValue"
					:options="selectOptions"
					label="Choose an option"
					item-text="label"
				/>
				<Select
					v-model="selectValue"
					:options="selectOptions"
					label="Disabled select"
					item-text="label"
					disabled
				/>
			</div>
		</Card>

		<Card title="Form Inputs" bg-surface2>
			<Header icon="i-mdi:slider" >
				Slider
			</Header>
			<div class="space-y-4">
				<Slider
					v-model="sliderValue"
					label="Volume"
				/>
				<p>Slider value: {{ sliderValue }}</p>
			</div>

			<Slider
				v-model="sliderValue"
				label="Volume"
				color="secondary"
			/>

			<Slider
				v-model="sliderValue"
				label="Volume"
				color="green"
			/>

			<Slider
				v-model="sliderValue"
				label="Volume"
				color="red"
			/>
		</Card>

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

		<Card title="Indicators" bg-surface2>
			<Header icon="i-fluent-mdl2:progress-ring-dots" >
				Progress
			</Header>
			<div class="flex items-center gap-4">
				<Spinner />
				<Spinner :progress="0.5" />
				<ProgressBar flex-1 />
				<ProgressBar flex-1 :progress="0.5" />
			</div>
		</Card>

		<Card title="Chips" bg-surface2>
			<Header icon="i-material-symbols:voting-chip" >
				Chip
			</Header>
			<div class="flex flex-wrap gap-2">
				<Chip>Default</Chip>
				<Chip color="primary">
					Primary
				</Chip>
				<Chip color="secondary">
					Secondary
				</Chip>
			</div>
		</Card>

		<Card title="Toggle" bg-surface2>
			<Header icon="i-mdi:toggle-switch-outline">
				Toggle
			</Header>
			<div class="flex flex-wrap items-center gap-2">
				<Toggle v-model="toggleValue" />
				<Toggle v-model="toggleValue" color="secondary" />
				<Toggle v-model="toggleValue" color="danger" />
				<Toggle v-model="toggleValue" disabled />
				<Toggle v-model="toggleValue" loading />
				<Toggle v-model="toggleValue" size="30px" />
			</div>
		</Card>

		<Card title="Color Picker" bg-surface2>
			<Header icon="i-mdi:palette">
				Color Picker
			</Header>
			<div class="flex flex-wrap items-center gap-2">
				<ColorPickerPop v-model:color="selectedColor" />
				<p>Selected color: {{ selectedColor }}</p>
			</div>

			<div class="flex flex-wrap items-center gap-2">
				<ColorPicker v-model:color="selectedColor" />
				<p>Selected color: {{ selectedColor }}</p>
			</div>

			<div class="mt-4 flex flex-wrap items-center gap-2">
				<ColorPickerPop v-model:color="selectedColor" simple />
				<p>Simple Pop: {{ selectedColor }}</p>
			</div>

			<div class="mt-4 flex flex-wrap items-center gap-2">
				<SimpleColorPicker v-model:color="selectedColor" />
				<p>Simple color picker: {{ selectedColor }}</p>
			</div>
		</Card>

		<Card title="Data Table" bg-surface2>
			<Header icon="i-mdi:table">
				Data Table
			</Header>
			<DataTable2 :headers="tableHeaders" :items="tableItems" />
		</Card>

		<Card title="Tooltips" bg-surface2>
			<Header icon="i-mdi:tooltip">
				Tooltips
			</Header>
			<div class="flex flex-wrap gap-2">
				<!-- Default (Bottom) -->
				<Button v-tooltip="'Default (Bottom)'">Default</Button>

				<!-- Explicit Top -->
				<Button v-tooltip.top="'Forced Top'">Top</Button>

				<!-- Explicit Left -->
				<Button v-tooltip.left="'Forced Left'">Left</Button>

				<!-- Explicit Right -->
				<Button v-tooltip.right="'Forced Right'">Right</Button>

				<Button
					color="primary"
					v-tooltip="{
						title: 'Rich Tooltip',
						text: 'This one has a title and text.',
						image: '/vite.svg',
						placement: 'bottom'
					}"
				>
					Rich Content (Bottom)
				</Button>

				<Button
					color="secondary"
					v-tooltip.top="{ html: '<b>Bold</b> text <span style=\'color: red\'>Support</span>' }"
				>
					HTML Content
				</Button>
			</div>
		</Card>

		<div h-50 />
	</div>
</template>
