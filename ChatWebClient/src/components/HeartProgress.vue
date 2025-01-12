<script>
  export default {
    name: 'HeartProgress',
    props: {
      value: {
        type: Number,
        default: 0,
        validator: (v) => v >= 0 && v <= 100,
      },
    },
    computed: {
      fillPercent() {
        const fillPercent = 100 - this.value; // Tính toán % chưa fill
        return `polygon(0% ${fillPercent}%, 100% ${fillPercent}%, 100% 100%, 0% 100%)`;
      },
    },
  };
</script>
<style scoped>
  .heart-progress-container {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 10px;
  }

  .heart-progress {
    width: 50px;
    height: 50px;
    position: relative;
  }

  .heart-bg {
    transition: fill 0.3s ease;
  }

  .heart-fill {
    transition: mask-position 0.3s ease;
  }

  .progress-text {
    font-size: 18px;
    fill: #333;
    font-weight: bold;
  }
  .heart-border {
    stroke: #ff0000; /* Màu viền đỏ */
    stroke-width: 3; /* Viền dày hơn */
  }
</style>

<template>
  <div class="heart-progress-container">
    <!--<svg xmlns="http://www.w3.org/2000/svg" version="1.1" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:svgjs="http://svgjs.dev/svgjs" viewBox="0 0 800 800">
      <g fill="hsl(356, 77%, 50%)" stroke-width="10" stroke="hsl(356, 77%, 40%)" id="heart">
      <path d="M404.18996358775917 218.15643303345712C627.653631141676 36.312849394776435 800 400 395.81005583906426 546.3687140990229 0 400 172.346368858324 36.312849394776435 404.18996358775917 218.15643303345712Z"
                                                                                                                                                                                                                                                            stroke-linecap="round" stroke-linejoin="round">
      </path>
      </g>
    </svg>-->
    <svg class="heart-progress" viewBox="0 0 800 800">
      <g fill="hsl(356, 77%, 50%)" stroke-width="10" stroke="hsl(356, 77%, 40%)" id="heart">
        <!-- Viền trái tim -->
        <path class="heart-border"
              d="M404.18996358775917 218.15643303345712C627.653631141676 36.312849394776435 800 400 395.81005583906426 546.3687140990229 0 400 172.346368858324 36.312849394776435 404.18996358775917 218.15643303345712Z" />
        <!-- Nền trái tim màu trắng -->
        <path class="heart-bg"
              d="M404.18996358775917 218.15643303345712C627.653631141676 36.312849394776435 800 400 395.81005583906426 546.3687140990229 0 400 172.346368858324 36.312849394776435 404.18996358775917 218.15643303345712Z"
              fill="#fff" />
        <!-- Phần tiến trình lấp đầy -->
        <path class="heart-fill"
              d="M404.18996358775917 218.15643303345712C627.653631141676 36.312849394776435 800 400 395.81005583906426 546.3687140990229 0 400 172.346368858324 36.312849394776435 404.18996358775917 218.15643303345712Z"
              :style="{ clipPath: fillPercent }"
              fill="#ff0000" />

      </g>
    </svg>
  </div>
</template>
