<script>
export default {
  data() {
    return {
      websocket: null, // Đối tượng WebSocket
      message: "", // Lưu trữ tin nhắn nhận được từ server
    };
  },
  methods: {
    connectWebSocket() {
      // Kết nối tới WebSocket server
      this.websocket = new WebSocket("wss://example.com/socket"); // Thay bằng URL WebSocket của bạn

      // Xử lý khi kết nối thành công
      this.websocket.onopen = () => {
        console.log("WebSocket connected!");
        this.websocket.send("Hello Server!"); // Gửi tin nhắn khi kết nối
      };

      // Xử lý khi nhận tin nhắn từ server
      this.websocket.onmessage = (event) => {
        console.log("Message received:", event.data);
        this.message = event.data; // Cập nhật message
      };

      // Xử lý khi WebSocket đóng
      this.websocket.onclose = () => {
        console.log("WebSocket disconnected.");
      };

      // Xử lý lỗi
      this.websocket.onerror = (error) => {
        console.error("WebSocket error:", error);
      };
    },
    sendMessage() {
      if (this.websocket && this.websocket.readyState === WebSocket.OPEN) {
        this.websocket.send("Message from Vue.js!");
      } else {
        console.warn("WebSocket is not connected.");
      }
    },
  },
  mounted() {
    this.connectWebSocket(); // Kết nối WebSocket khi component được mount
  },
  beforeDestroy() {
    if (this.websocket) {
      this.websocket.close(); // Đóng WebSocket khi component bị hủy
    }
  },
};
</script>
<template>
    <main class="main">
        <div class="container h-100">

            <div class="d-flex flex-column h-100 justify-content-center text-center">
                <div class="mb-6">
                    <span class="icon icon-xl text-muted">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-message-square"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path></svg>
                    </span>
                </div>

                <p class="text-muted">Pick a person from left menu, <br> and start your conversation.</p>
            </div>

        </div>
    </main>
</template>