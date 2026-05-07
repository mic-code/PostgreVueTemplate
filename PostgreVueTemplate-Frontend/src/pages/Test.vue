<script setup lang="ts">
import { useRouter } from "vue-router";
import { Signout } from "../services/authService";
import { IsUp, IsUpAuth, IsUpClaim } from "../services/testService";

const router = useRouter();

function handleIsUp()
{
	console.log("test");
	IsUp();
}

function handleIsUpAuth()
{
	IsUpAuth();
}

function handleIsUpClaim()
{
	IsUpClaim();
}

function handleSignout()
{
	Signout();
	router.push("/signin");
}

</script>

<template>
	<Button @click="handleIsUp">IsUp</Button>
	<Button @click="handleIsUpAuth">IsUpAuth</Button>
	<Button @click="handleIsUpClaim">IsUpClaim</Button>
	<Button @click="handleSignout">Signout</Button>

	<div style="width: 100px; height: 100px;" bg-green shadow-xl>

	</div>

	<div style="width: 100px; height: 100px; box-shadow: 10px 5px 5px;">

	</div>

</template>