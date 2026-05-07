<script setup lang="ts">
import { computed, PropType, ref, useSlots } from "vue";

const props = defineProps({
	headers: { type: Object, default: null },
	items: {
		type: Array as PropType<object[]>,
		required: true,
		default: () => [],
	},
	selectedItem: {
		type: Object,
		default: null
	}
});

const emit = defineEmits(["row-click"]);

const sortKey = ref<string | null>(null);
const sortOrder = ref<"asc" | "desc">("asc");
const slots = useSlots();
const hasRowSlot = computed(() => !!slots.row);

const finalHeaders = computed(() =>
{
	if(props.headers)
		return props.headers;

	if (props.items.length === 0)
		return [];

	// Use the keys from the first object as headers
	const h = Object.keys(props.items[0]);
	console.log(h);
	return h;
});

const sortedItems = computed(() =>
{
	if (!sortKey.value)
	{
		return props.items;
	}

	const key = sortKey.value;
	const order = sortOrder.value === "asc" ? 1 : -1;

	// Create a copy to avoid mutating the original prop
	return [...props.items].sort((a, b) =>
	{
		const valA = a[key];
		const valB = b[key];

		if (valA < valB) return -1 * order;
		if (valA > valB) return 1 * order;
		return 0;
	});
});

function sortBy(key: string)
{
	if (sortKey.value === key)
	{
		sortOrder.value = sortOrder.value === "asc" ? "desc" : "asc";
	}
	else
	{
		sortKey.value = key;
		sortOrder.value = "asc";
	}
}

function isObject(value: any)
{
	return value !== null && typeof value === "object" && !Array.isArray(value);
}

function formatValue(value: any)
{
	if (isObject(value))
	{
		return JSON.stringify(value);
	}
	return value;
}

function handleRowClick(item: any)
{
	emit("row-click", item);
}
</script>

<template>
	<div class="data-table-container">
		<table v-if="items.length > 0" class="data-table">
			<thead>
				<tr>
					<th v-for="header in finalHeaders" :key="header" @click="sortBy(header)" class="sortable-header">
						{{ header }}
						<span v-if="sortKey === header" class="sort-arrow">
							{{ sortOrder === 'asc' ? '▲' : '▼' }}
						</span>
					</th>
				</tr>
			</thead>
			<tbody>
				<tr v-for="(item, index) in sortedItems" :key="index" @click="handleRowClick(item)" :class="{ 'selected-row': selectedItem === item }">
					<slot v-if="hasRowSlot" name="row" :item="item" :headers="finalHeaders">

					</slot>
					<td v-else v-for="header in finalHeaders" :key="header">
						{{ formatValue(item[header]) }}
					</td>
				</tr>
			</tbody>
		</table>
		<div v-else class="no-data">
			<p>No data to display.</p>
		</div>
	</div>
</template>

<style scoped>
.data-table-container {
	width: 100%;
	overflow-x: auto;
	border: 1px solid #444;
	border-radius: 4px;
}

.data-table {
	width: 100%;
	border-collapse: collapse;
	text-align: left;
}

.data-table th,
.data-table td {
	padding: 12px 15px;
	border-bottom: 1px solid #444;
}

.data-table tbody tr:last-child td {
	border-bottom: none;
}

.data-table tbody tr:hover {
	background-color: var(--surfacehighlight);
	cursor: pointer;
}

.data-table thead {
	background-color: var(--surface3);
}

.data-table th {
	font-weight: bold;
	text-transform: capitalize;
}

.sortable-header {
	cursor: pointer;
	user-select: none;
}

.sortable-header:hover {
	background-color: var(--surfacehighlight);
}

.sort-arrow {
	margin-left: 5px;
	font-size: 0.8em;
}

.no-data {
	text-align: center;
	padding: 20px;
	color: var(--onsurface);
}

.selected-row {
	background-color: var(--primary);
	color: var(--onprimary);
}
</style>
