import { createApp } from "vue";
import { createPinia } from "pinia";
import "./style.css";
import "element-plus/dist/index.css";
import ElementPlus from "element-plus";
import App from "./App.vue";

const pinia = createPinia();
const app = createApp(App);

app.use(pinia);
app.use(ElementPlus);

app.mount("#app");
