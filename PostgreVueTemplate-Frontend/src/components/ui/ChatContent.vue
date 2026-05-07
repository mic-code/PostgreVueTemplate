<script setup lang="ts">
import { PropType, ref, watch, nextTick, onMounted, computed } from "vue";
import { AIOperation, ChatMessage } from "../../services/chatService";
import { useAppStore } from "../../stores/appStore";
import MarkdownIt from "markdown-it";

const md = new MarkdownIt({ breaks: true, linkify: true, html: true });

const props = defineProps({
	chatMessages: Array as PropType<Array<ChatMessage>>,
	chatOwner: String,
	context: Object,
	isSending: Boolean,
	showToolCall: { type: Boolean, default: true },
	showToolResult: { type: Boolean, default: true },
	hideThinkOnly: { type: Boolean, default: true },
});

const appStore = useAppStore();
const showLoading = computed(() =>
{
	if (!props.isSending) return false;
	if (!props.chatMessages || props.chatMessages.length === 0) return true;

	const lastMsg = props.chatMessages[props.chatMessages.length - 1];
	if ((lastMsg.AuthorName === "AI" || lastMsg.AuthorName === "Assistant") && HasVisibleContent(lastMsg))
	{
		return false;
	}
	return true;
});
const model = defineModel<string>();
const chatArea = ref();

function handleWindowOpen()
{
	appStore.aiOpen = true;
	window.addEventListener("click", onPointerClick);
}

function onPointerClick(e: MouseEvent)
{
	if (chatArea.value && !chatArea.value.contains(e.target as Node))
	{
		window.removeEventListener("click", onPointerClick);
		appStore.aiOpen = false;
	}
}

const emit = defineEmits<{
	onSend: [ content: string],
	onStop: []
}>();

const chatContainer = ref<HTMLElement | null>(null);
const formRef = ref<HTMLFormElement | null>(null);

function scrollToBottom()
{
	nextTick(() =>
	{
		if (chatContainer.value)
		{
			chatContainer.value.scrollTop = chatContainer.value.scrollHeight;
		}
	});
}

watch(() => props.chatMessages, scrollToBottom, { deep: true });
watch(() => props.isSending, scrollToBottom);

onMounted(scrollToBottom);


async function handleQuery()
{
	if (props.isSending) return;
	
	// Trigger native form validation
	if (formRef.value && !formRef.value.checkValidity())
	{
		formRef.value.reportValidity();
		return;
	}
	
	if (!model.value) return;
	emit("onSend", model.value);
	model.value = "";
}

function handleStop()
{
	emit("onStop");
}

function handleAppendClick()
{
	if (props.isSending)
	{
		handleStop();
	}
	else
	{
		handleQuery();
	}
}

const appendIcon = computed(() => props.isSending ? "i-mdi:stop" : "i-mdi:arrow-right");

async function handleCopyContext()
{
	if (!props.context)
		return;

	console.log(props.context);

	try
	{
		await navigator.clipboard.writeText(JSON.stringify(props.context, null, 2));
	}
	catch (err)
	{
		console.error("Failed to copy context to clipboard:", err);
	}
}

function GetAIColor(message: ChatMessage)
{
	// return message.IsError ? "bg-red text-oncolor font-bold" : "bg-surfacehighlight";
	return "bg-surfacehighlight";
}

function GetOperations(operations: AIOperation[])
{
	if (!operations) return [];

	const opMap = operations.reduce((acc, op) =>
	{
		if (!acc[op.type])
		{
			acc[op.type] = { type: op.type, count: 0, operations: [] };
		}
		acc[op.type].count++;
		acc[op.type].operations.push(op);
		return acc;
	}, {} as Record<string, { type: string; count: number; operations: AIOperation[] }>);

	return Object.values(opMap);
}

function GetMessageText(message: ChatMessage)
{
	if (!message.Contents) return "";
	return message.Contents.filter(c => c.Text !== undefined).map(c => c.Text).join("");
}

function IsText(content: any)
{
	return content.text !== undefined || content.Text !== undefined;
}

function IsToolCall(content: any)
{
	return (content.name !== undefined || content.Name !== undefined) &&
           (content.callId !== undefined || content.CallId !== undefined);
}

function IsToolResult(content: any)
{
	return (content.result !== undefined || content.Result !== undefined) &&
           (content.callId !== undefined || content.CallId !== undefined);
}

function IsError(content: any)
{
	return content.Error === true || content.error === true;
}

function HasVisibleContent(message: ChatMessage)
{
	if (!message.Contents) return false;

	return message.Contents.some(content =>
	{
		if (IsError(content)) return true;
		if (IsText(content))
		{
			const val = GetText(content);
			const { think, text } = splitThink(val);
			return !!text || (!props.hideThinkOnly && !!think);
		}
		if (IsToolCall(content)) return props.showToolCall;
		if (IsToolResult(content)) return props.showToolResult;
		return false;
	});
}

function GetText(content: any): string
{
	return content.text || content.Text || "";
}

function GetToolName(content: any): string
{
	return content.name || content.Name || content.toolName || content.ToolName || "";
}

function GetCallId(content: any): string
{
	return content.callId || content.CallId || "";
}

function GetArguments(content: any): any
{
	return content.arguments || content.Arguments || {};
}

function GetResult(content: any): any
{
	return content.result !== undefined ? content.result : content.Result;
}

function splitThink(text: string)
{
	const thinkStartTag = "<think>";
	const thinkEndTag = "</think>";

	const start = text.indexOf(thinkStartTag);
	if (start === -1) return { think: "", text };

	const end = text.indexOf(thinkEndTag, start);
	if (end === -1)
	{
		// Still thinking...
		return { think: text.substring(start + thinkStartTag.length).trim(), text: "" };
	}

	const think = text.substring(start + thinkStartTag.length, end).trim();
	const remaining = (text.substring(0, start) + text.substring(end + thinkEndTag.length)).trim();
	return { think, text: remaining };
}

</script>

<template>
	<div fixed bottom-0 class="left-1/2" :class="appStore.aiOpen?'pointer-events-auto':'pointer-events-none'" style="transform: translateX(-50%); padding-bottom: env(safe-area-inset-bottom);">
		<div rounded="0.5rem" ref="chatArea" :class="appStore.aiOpen?'chatWindow':''" relative flex flex-col style="width: 600px; max-height: 60vh; max-width: 100vw;" >
			<div :style="[appStore.aiOpen?'':'visibility:hidden']" grow v-if="!chatMessages || chatMessages.length === 0" >
				No messages yet.
			</div>
			<div v-else :style="[appStore.aiOpen?'':'visibility:hidden']" ref="chatContainer" w-full grow overflow-y-auto>
				<template v-for="message in chatMessages" :key="message.Id">
					<div v-if="HasVisibleContent(message)" :class="message.AuthorName=='User'?'ms-a bg-primary text-oncolor':GetAIColor(message)" style="max-width: 85%;" mx-2 mt-2 w-fit rounded-2xl >
						<div p-2>
							<template v-for="(content, idx) in message.Contents" :key="idx">
								<template v-if="IsText(content)">
									<div v-if="splitThink(GetText(content)).think" class="text-xs">
										<details>
											<summary class="non-italic flex cursor-pointer list-none items-center gap-1 p-2 font-bold transition-colors hover:bg-surface3/30">
												<Icon icon="i-mdi:brain" size="1rem" />
												<Icon icon="i-mdi:chevron-down" size="1rem" class="ms-a" />
											</summary>
											<div class="px-2 pb-2">
												{{ splitThink(GetText(content)).think }}
											</div>
										</details>
									</div>
									<div v-if="splitThink(GetText(content)).text" class="markdown-body ma-1" v-html="md.render(splitThink(GetText(content)).text)"></div>
								</template>
								<div v-else-if="IsToolCall(content) && showToolCall" class="my-1 border-s-4 border-primary rounded bg-primary/10 p-2 text-xs text-oncolor">
									<div flex items-center gap-1 font-bold>
										<Icon icon="i-mdi:tools" size="1rem" />
										{{ GetToolName(content) }}
									</div>
									<pre class="mt-1 overflow-x-auto opacity-70">{{ JSON.stringify(GetArguments(content), null, 2) }}</pre>
								</div>
								<div v-else-if="IsToolResult(content) && showToolResult" class="my-1 border-s-4 border-success rounded bg-success/10 p-2 text-xs text-oncolor">
									<div flex items-center gap-1 font-bold>
										<Icon icon="i-mdi:check-circle" size="1rem" />
										{{ GetToolName(content) || 'Tool' }}
									</div>
									<pre class="ma-0 overflow-x-auto opacity-70">{{ typeof GetResult(content) === 'string' ? GetResult(content) : JSON.stringify(GetResult(content), null, 2) }}</pre>
								</div>
								<div v-else-if="IsError(content)" class="my-1 border-s-4 border-red-500 rounded bg-red-500/10 p-2 text-xs">
									<div flex items-center gap-1 font-bold text-red-500>
										<Icon icon="i-mdi:alert-circle" size="1rem" />
										Error
									</div>
									<pre class="ma-0 overflow-x-auto text-red-400">{{ GetText(content) }}</pre>
								</div>
							</template>
						</div>
					</div>
				</template>
				<div v-if="showLoading" class="cm bg-surfacehighlight" mx-2 mt-2 w-fit rounded-2xl p-2>
					<Spinner :size="24" />
				</div>
			</div>
			<div pointer-events-auto m-2 flex-none >
				<form ref="formRef" @submit.prevent="handleQuery">
					<TextField
						v-model="model"
						density="compact"
						:appendIcon="appendIcon"
						:placeholder="$t('chatWithAI')"
						variant="outlined"
						aria-label="query"
						round="1rem"
						required
						show-submit
						bg-surface
						append-button-type="button"
						@click="handleWindowOpen"
						@click:append="handleAppendClick"
						@keydown.stop
						@keyup.stop
					>
						<template #prepend>
							<Icon icon="i-tabler:ai" rounded-4 bg-black color="white" @click="handleCopyContext"/>
						<!-- <Button icon="i-mdi-microphone" variant="flat"></Button> -->
						</template>
					</TextField>
				</form>
			</div>
		</div>
	</div>
</template>
<style scoped>
.chatWindow {
	background-color: rgb(from var(--surface4) r g b / 60%);
	box-shadow: -10px -10px 10px rgb(0 0 0 / 10%);
}

details[open] .details-icon {
	transform: rotate(180deg);
}

summary::-webkit-details-marker {
	display: none;
}

.markdown-body {
	line-height: 1.5;
}

.markdown-body :deep(h1),
.markdown-body :deep(h2),
.markdown-body :deep(h3),
.markdown-body :deep(h4),
.markdown-body :deep(h5),
.markdown-body :deep(h6) {
	font-weight: bold;
	margin-top: 1em;
	margin-bottom: 0.5em;
	line-height: 1.25;
}
.markdown-body :deep(h1) { font-size: 1.5em; }
.markdown-body :deep(h2) { font-size: 1.25em; }

.markdown-body :deep(p) {
	margin-bottom: 0.5em;
}

.markdown-body :deep(p):last-child {
	margin-bottom: 0;
}

.markdown-body :deep(ul), .markdown-body :deep(ol) {
	margin-bottom: 1em;
	padding-left: 1.5em;
	list-style: inherit;
}
.markdown-body :deep(ul) { list-style-type: disc; }
.markdown-body :deep(ol) { list-style-type: decimal; }

.markdown-body :deep(pre) {
	background-color: rgb(from var(--surface3) r g b / 50%);
	padding: 0.75em;
	border-radius: 0.5rem;
	overflow-x: auto;
	margin-bottom: 1em;
	font-family: monospace;
}

.markdown-body :deep(code) {
	font-family: monospace;
	background-color: rgb(from var(--surface3) r g b / 50%);
	padding: 0.2em 0.4em;
	border-radius: 0.25rem;
}

.markdown-body :deep(pre) :deep(code) {
	background-color: transparent;
	padding: 0;
}

.markdown-body :deep(a) {
	color: var(--primary);
	text-decoration: underline;
}

.markdown-body :deep(blockquote) {
	border-left: 4px solid var(--surface3);
	padding-left: 1em;
	margin-left: 0;
	font-style: italic;
}
</style>
