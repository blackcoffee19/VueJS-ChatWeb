<script setup lang="js">
import Card from 'primevue/card';
import DataView from 'primevue/dataview';
import Avatar from 'primevue/avatar';
import { ref, onMounted } from "vue";
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import InputText from 'primevue/inputtext';

const search = ref();

// onMounted(() => {
//     ProductService.getProductsSmall().then((data) => (products.value = data.slice(0, 5)));
// });

const products = ref([
{
    id: '1',
    code: 'f230fh0g3',
    name: 'HinataSohara',
    lastmess: 'Hi Im HINATA',
    image: 'hinata-50.jpg',
    rating: 5,
    spantime: "1 minute ago"
},
{
    id: '2',
    code: 'f230fh0g4',
    name: 'Juria',
    lastmess: 'BOOM',
    image: 'juria-1.jpeg',
    rating: 5,
    spantime: "30 minutes ago"
},
{
    id: '3',
    code: 'f230fh0g5',
    name: 'Jurin',
    lastmess: 'Hi Im Jurin',
    image: 'jurin-1.jpeg',
    rating: 5,
    spantime: "1 hour ago"
},
{
    id: '4',
    code: 'f230fh0g6',
    name: 'Chisa',
    lastmess: 'Hi Im CHISA',
    image: 'chisa-1.jpeg',
    rating: 5,
    spantime: "2 hours ago"
},
{
    id: '5',
    code: 'f230fh0g7',
    name: 'Maya',
    lastmess: 'Hi Im Martin',
    image: 'maya-1.jpeg',
    rating: 5,
    spantime: "3 hours ago"
},
{
    id: '6',
    code: 'f230fh0g9',
    name: 'Harvey',
    lastmess: 'Hi Im Harvey',
    image: 'harvey-1.jpeg',
    rating: 5,
    spantime: "1 day ago"
},
{
    id: '7',
    code: 'f230fh0g8',
    name: 'Cocona',
    lastmess: 'Hi Im Coconut  KAnsdnalsk ksnalasnd snsdkaldn askndnasl kndls adna naskdla badsbk',
    image: 'cocona-1.jpeg',
    rating: 5,
    spantime: "2 days ago"
},
]);

</script>
<style lang="css" module>
.myinput {
    border-radius: 2rem;
    padding: 1rem 2rem;
    border-width: 2px;
}
</style>
<template>
    <aside class=" overflow-auto lg:w-4 xl:w-3">
        <div class="tablist" role="tablist">
            <!-- Chats -->
            <div class="tab-pane fade show active h-screen bg-blue-200" id="tab-content-chats" role="tabpanel">
                <div class="flex flex-column">
                    <div class="chats hide-scrollbar">
                        <div class="container ">
                            <!-- Title -->
                            <div class="flex flex-column mb-3 ">
                                <h2>Chats</h2>
                               
                                <IconField >
                                    <InputIcon class="pi pi-search" />
                                    <InputText v-model="value1" :class="$style.myinput" placeholder="Search" fluid />
                                </IconField>

                            </div>
                            <!-- Chats -->
                            <DataView :value="products">
                                <template #list="slotProps">
                                    <div v-for="(item, index) in slotProps.items" :key="index">
                                        <Card class="mb-2  overflow-hidden" style="height: 100px;">
                                            <template #content>
                                                    <div class="flex flex-col sm:items-center p-1 gap-2 " :class="{ 'border-t border-surface-500 dark:border-surface-700': index !== 0 }">
                                                        <div class="w-3relative">
                                                            <Avatar :image="`/images/${item.image}`"size="xlarge" shape="circle" class="flex items-center justify-center mr-2" />
                                                        </div>
                                                        <div class="flex flex-column justify-around items-start w-9">
                                                            <div class="flex-grow-1 line-height-1 overflow-hidden" style="height: 50px;">
                                                                <span class="font-medium text-surface-500 dark:text-surface-400 text-sm">{{ item.name }}</span>
                                                                <div class="text-lg font-medium mt-2 overflow-hidden">{{ item.lastmess }}</div>
                                                            </div>
                                                            <div class="text-right">
                                                            <span class="text-surface-900 font-medium text-sm">{{ item.spantime }}</span>
                                                            </div>
                                                        </div>
                                                        
                                                    </div>
                                            </template>
                                        </Card>
                                    </div>
                                </template>
                            </DataView>
                        <!-- Chats -->
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </aside>
</template>