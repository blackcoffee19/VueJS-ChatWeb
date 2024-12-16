<script setup lang="ts">
import Navigation from '@/components/Navigation.vue';
</script>

<template>
    <div class="layout overflow-hidden">
        <!-- Navigation -->
        <Navigation/>
        <!-- Navigation -->
        <router-view></router-view>
    </div>
</template>
<style>
.layout{
    display: flex;
    flex-direction: row;
    height: 100%;
}
</style>