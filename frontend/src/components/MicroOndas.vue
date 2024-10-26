<script setup>
  import { ref, watch } from 'vue'
  import axios from "axios";

  let apiUrl = "http://localhost:5169/"
  let intervalId;

  const apiResponse = ref("")

  const tempo = ref("30")
  const potencia = ref("10")
  const erros = ref([])

  const emExecucao = ref(false)
  const estaPausado = ref(false)
  const forno = ref("")

  function ehInteiro(str) {
    // alert(str)
    str = str.trim();
    if (!str) {
        return false;
    }
    str = str.replace(/^0+/, "") || "0";
    var n = Math.floor(Number(str));
    return (n !== Infinity) && (String(n) === str) && (n >= 0);
  }

  function converterTempo(segundos) {
    const minutos = Math.floor(segundos / 60);
    const restanteSegundos = segundos % 60;
    return `${minutos}:${String(restanteSegundos).padStart(2, '0')}`;
  }

  function chamarAPI(endpoint) {

    console.log("chamarAPI")

    axios
      .get(apiUrl + endpoint)
      .then((resp) => (apiResponse.value = resp.data))
      .catch(error => console.log(error))
    
  }

  function iniciar() {

    if (emExecucao.value) {

      chamarAPI("iniciar")

      if (estaPausado.value) {
        intervalId = setInterval(
        () => {
          chamarAPI("status")
        },
        1000);
      // estaPausado.value = false
      }

    } else {

      let tempoInt = -1

      erros.value = []

      if (potencia.value.trim() == '')
        potencia.value = "10"
      if ((!ehInteiro(potencia.value)) || (Number(potencia.value) < 1) || (Number(potencia.value) > 10))
        erros.value.push("A potência deve ser um número inteiro entre 1 e 10")

      if (tempo.value.trim() == '')
        tempo.value = "30"
      if (ehInteiro(tempo.value))
        tempoInt = Number(tempo.value)
      if ((tempoInt < 1) || (tempoInt > 120))
        erros.value.push("O tempo deve ser um número inteiro entre 1 e 120")

      if (erros.value.length)
        return false

      if (tempoInt >= 60)
        tempo.value = converterTempo(tempoInt)

      chamarAPI("iniciar?potencia=" + potencia.value + "&tempo=" + tempo.value)
      intervalId = setInterval(
        () => {
          chamarAPI("status")
        },
        1000);
    }
  }

  function pausarOuCancelar() {

    if (emExecucao.value) {

      chamarAPI("pausarOuCancelar")

    }

  }

  watch(apiResponse, async (newResp, oldResp) => {

    console.log(JSON.stringify(newResp))

    emExecucao.value = newResp.status != "INATIVO"
    estaPausado.value = newResp.status == "PAUSADO"

    forno.value = newResp.alimento
    let tempoInt = newResp.tempoRestante
    if (tempoInt >= 60)
        tempo.value = converterTempo(tempoInt)
    else
        tempo.value = String(newResp.tempoRestante)

    if (!emExecucao.value || estaPausado.value) {
      console.log("clearInterval")
      clearInterval(intervalId)
    }

})

</script>

<template>
  <div id="app">
    <!-- <form @submit.prevent="iniciar"> -->
    <!-- <form> -->

      <div v-if="erros.length">
        <b>Por favor, corrija o(s) seguinte(s) erro(s):</b>
        <ul>
          <li v-for="erro in erros">{{ erro }}</li>
        </ul>
      </div>

      <div class="form-group">
        <label for="tempo">Tempo (segundos):</label>
        <input
          id="tempo"
          type="text"
          v-model="tempo"
          :disabled="emExecucao"
        />
      </div>
      
      <div class="form-group">
        <label for="potencia">Potência (1 a 10):</label>
        <input
          id="potencia"
          type="text"
          v-model="potencia"
          :disabled="emExecucao"
        />
      </div>
      
      <button v-on:click="iniciar">Iniciar</button>
      <button class="pausar-cancelar" v-on:click="pausarOuCancelar">
        {{ estaPausado ? "Cancelar" : "Pausar" }}
      </button>
    <!-- </form> -->
    
    <!-- <div v-if="emExecucao"> -->
    <div>
      <p>{{ forno }}</p>
    </div>
  </div>
</template>

<style scoped>
#app {
  max-width: 400px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 10px;
  box-shadow: 2px 2px 12px rgba(0,0,0,0.1);
}

h1 {
  text-align: center;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
}

ul li {
  text-align: left;
}

input {
  width: 100%;
  padding: 8px 20px;
  box-sizing: border-box;
  text-align: right;
  font-size: 20px;
}

button {
  width: 100%;
  margin: 10px 0px;
  padding: 10px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

button.pausar-cancelar {
  background-color: #bd8736;
}

button.pausar-cancelar:hover {
  background-color: #b67735;
}
</style>
