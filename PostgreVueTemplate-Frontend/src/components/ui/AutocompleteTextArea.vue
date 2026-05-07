<template>
	<div relative>
		<!-- Suggestions Popup -->
		<div v-if="showSuggestions" ref="suggestionsContainer" class="absolute bottom-full left-0 z-10 mb-0.5 w-full border border-gray-500 rounded-md shadow-lg" style="background-color: var(--surface); color: var(--onsurface);">
			<div v-for="(suggestion, index) in suggestions" :key="suggestion.text"
				class="group flex cursor-pointer items-center justify-between px-1 py-0.5 text-xs"
				:style="index === selectedIndex ? 'background-color: var(--surfacehighlight);' : ''"
				@click="selectSuggestion(suggestion)"
				@mouseenter="selectedIndex = index">
				<span class="mr-1 flex-1 truncate">{{ suggestion.text }}</span>
				<div class="flex items-center gap-0.5">
					<span class="shrink-0 text-[10px] text-gray-500">{{ suggestion.type }}</span>
					<Button
						v-if="suggestion.type === 'history'"
						icon="i-mdi:close"
						:size="0.7"
						variant="text"
						color="error"
						class="opacity-100 transition-opacity md:opacity-0 md:group-hover:opacity-100"
						@click.stop="removeHistoryItem(suggestion.text)"
					/>
				</div>
			</div>
		</div>

		<TextArea
			:modelValue="modelValue"
			@update:modelValue="updateValue"
			@keydown="handleKeyDown"
			:placeholder="placeholder"
			:rows="rows"
			:autoExpand="autoExpand"
		/>
	</div>
</template>

<script setup lang="ts">
import { ref, watch, computed, nextTick } from "vue";
import TextArea from "./TextArea.vue";
import { getHistory, deleteInput } from "../../utilities/inputHistory";
import Button from "./Button.vue";

const props = withDefaults(defineProps<{
	modelValue: string;
	repoMap: string | null;
	placeholder?: string;
	rows?: number;
	autoExpand?: boolean;
}>(), {
	rows: 1,
	autoExpand: true
});

const emit = defineEmits(["update:modelValue", "submit"]);


const updateValue = (val: string) =>
{
	emit("update:modelValue", val);
};

interface Suggestion {
  text: string;
  type: "file" | "history" | "symbol";
}

const suggestions = ref<Suggestion[]>([]);
const showSuggestions = ref(false);
const selectedIndex = ref(0);

const parsedRepoMap = computed(() =>
{
	if (!props.repoMap || typeof props.repoMap !== "string") return [];
	const symbols = new Set<string>();
	const lines = props.repoMap.split("\n");

	const ignoreSet = new Set([
		"void", "int", "bool", "string", "var", "object", "class", "interface", "struct", "enum", "record",
		"async", "await", "task", "public", "private", "protected", "internal", "static", "readonly",
		"return", "using", "namespace", "new", "null", "true", "false", "if", "else", "for", "foreach", "while",
		"do", "switch", "case", "break", "continue", "try", "catch", "finally", "throw", "lock", "const", "event",
		"delegate", "override", "virtual", "abstract", "sealed", "extern", "volatile", "unsafe", "fixed", "stackalloc",
		"operator", "implicit", "explicit", "this", "base", "params", "ref", "out", "in", "is", "as", "sizeof", "typeof",
		"checked", "unchecked", "default", "value", "get", "set", "add", "remove", "where", "from", "select", "group", "by",
		"into", "orderby", "join", "let", "on", "equals", "descending", "ascending", "dynamic", "global", "alias", "yield", "partial"
	]);

	for (const line of lines)
	{
		const trimmed = line.trim();
		if (!trimmed) continue;

		// 1. Existing logic for file paths (handles tree structure)
		const cleanLine = line.replace(/[│├└─\s]/g, "");
		if (cleanLine && cleanLine.includes("."))
		{
			symbols.add(cleanLine);
		}

		// 2. Extract words/symbols
		const matches = line.matchAll(/\b[a-zA-Z_][a-zA-Z0-9_]*\b/g);
		for (const match of matches)
		{
			const word = match[0];
			if (word.length < 2) continue;

			if (ignoreSet.has(word.toLowerCase())) continue;

			symbols.add(word);
		}
	}
	return Array.from(symbols);
});

const updateSuggestions = (val: string, resetIndex: boolean = false) =>
{
	const lastWordMatch = val.match(/(\S+)$/); // Last word
	const lastWord = lastWordMatch ? lastWordMatch[1] : "";

	let newSuggestions: Suggestion[] = [];

	// History suggestions
	if (val.trim().length >= 1)
	{
		const history = getHistory();
		const matches = history
			.filter(item => item.count > 1 && item.text.toLowerCase().startsWith(val.trim().toLowerCase()) && item.text !== val.trim())
			.sort((a, b) => b.count - a.count)
			.slice(0, 10)
			.map(item => ({ text: item.text, type: "history" as const }));

		newSuggestions.push(...matches);
	}

	// File/Symbol suggestions
	if (lastWord.length >= 2 && parsedRepoMap.value && parsedRepoMap.value.length > 0)
	{
		const query = lastWord.toLowerCase();
		const matches = parsedRepoMap.value
			.filter(item => item.toLowerCase().includes(query))
			.slice(0, 50)
			.map(item => ({
				text: item,
				type: (item.includes(".") || item.includes("/")) ? "file" as const : "symbol" as const
			}));
		newSuggestions.push(...matches);
	}

	if (newSuggestions.length > 0)
	{
		// Limit to best 10
		if (newSuggestions.length > 10)
		{
			newSuggestions = newSuggestions.slice(0, 10);
		}

		// Reverse so best matches (first in list) appear at the bottom (closest to input)
		newSuggestions.reverse();

		suggestions.value = newSuggestions;
		showSuggestions.value = true;
		if (resetIndex)
		{
			selectedIndex.value = suggestions.value.length - 1;
		}
		else if (selectedIndex.value >= suggestions.value.length)
		{
			selectedIndex.value = Math.max(0, suggestions.value.length - 1);
		}
	}
	else
	{
		showSuggestions.value = false;
		suggestions.value = [];
	}
};

watch(() => props.modelValue, (val) =>
{
	updateSuggestions(val, true);
});

const removeHistoryItem = (text: string) =>
{
	deleteInput(text);
	updateSuggestions(props.modelValue, false);
};

const selectSuggestion = (suggestion: Suggestion) =>
{
	if (suggestion.type === "file" || suggestion.type === "symbol")
	{
		const newValue = props.modelValue.replace(/(\S+)$/, `\`${suggestion.text}\` `);
		emit("update:modelValue", newValue);
	}
	else
	{
		// Replace whole text for history
		emit("update:modelValue", suggestion.text);
	}
	showSuggestions.value = false;
};

const handleKeyDown = (e: KeyboardEvent) =>
{
	if (showSuggestions.value)
	{
		if (e.key === "ArrowUp")
		{
			e.preventDefault();
			selectedIndex.value = (selectedIndex.value - 1 + suggestions.value.length) % suggestions.value.length;
			return;
		}
		else if (e.key === "ArrowDown")
		{
			e.preventDefault();
			selectedIndex.value = (selectedIndex.value + 1) % suggestions.value.length;
			return;
		}
		else if (e.key === "Tab")
		{
			e.preventDefault();
			selectSuggestion(suggestions.value[selectedIndex.value]);
			return;
		}
		else if (e.key === "Escape")
		{
			showSuggestions.value = false;
			return;
		}
	}

	if (e.key === "Enter" && !e.shiftKey)
	{
		e.preventDefault();
		showSuggestions.value = false;
		emit("submit");
	}
};
</script>
