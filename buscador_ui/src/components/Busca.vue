<template>
  <div>
    <div class="barraBusca">
      <nav class="navbar navbar-light">
        <a class="navbar-brand">Compare preços...</a>
        <div class="form-inline">
          <input
            v-model="termoBusca"
            class="form-control mr-sm-2 input-lg"
            type="search"
            placeholder="Digite aqui a categoria"
            aria-label="Search"
            style="width:500px"
          />
          <button
            class="btn btn-outline-primary my-2 my-sm-0"
            type="submit"
            @click="realizarBusca()"
            :disabled="termoBusca.length < 1"
          >
            Comparar preços
          </button>
        </div>
      </nav>
    </div>

    <div class="py-5">
      <div class="container">
        <div class="row hidden-md-up">
          <div
            class="col-lg-4 col-md-6 col-sm-12 mb-3"
            v-for="item in consulta.resultados"
            :key="item.id"
          >
            <div class="card" style="width: 20rem;">
              <img
                :src="item.urlImagem"
                class="card-img-top"
                alt=""
                style="padding:5px;width:150px;height: auto;margin: auto;"
              />
              <div class="card-body">
                <h5 class="card-title">
                  <div v-if="item.nome.length < limiteCaracteresNome">
                    {{ item.nome }}
                  </div>
                  <div v-else :title="item.nome">
                    {{ item.nome.substring(0, limiteCaracteresNome) + ".." }}
                  </div>

                  <!-- {{ item.nome }} -->
                </h5>
                <div class="card-text">
                  <div v-if="item.descricao.length < limiteCaracteresDescricao">
                    {{ item.descricao }}
                  </div>
                  <div v-else :title="item.descricao">
                    {{
                      item.descricao.substring(0, limiteCaracteresDescricao) +
                        ".."
                    }}
                  </div>
                </div>

                <ul class="list-group list-group-flush">
                  <li class="list-group-item">
                    {{ item.categoria }} ({{ item.origem }})
                  </li>
                  <li class="list-group-item">Tamanhos: {{ item.tamanhos }}</li>
                  <li class="list-group-item">{{ item.preco }}</li>
                </ul>
                <a
                  :href="item.urlProduto"
                  class="btn btn-primary m-3"
                  target="_blank"
                  >Ir à loja
                </a>
              </div>
            </div>
          </div>
        </div>
        <nav
          aria-label="Page navigation"
          v-show="consulta.resultados.length > 0"
        >
          <hr />
          <ul class="pagination justify-content-center m-4">
            <li class="page-item mr-5">
              <button
                class="page-link"
                @click="irParaPaginaAnterior()"
                tabindex="-1"
                :disabled="!consulta.possuiPaginaAnterior"
              >
                Anterior
              </button>
            </li>

            <li class="page-item" v-show="paginaAtual > 2">
              <button class="page-link" @click="irParaPrimeiraPagina()">
                1
              </button>
            </li>

            <li class="page-item" v-show="consulta.possuiPaginaAnterior">
              <button class="page-link" @click="irParaPaginaAnterior()">
                {{ paginaAtual - 1 }}
              </button>
            </li>

            <li class="page-item active">
              <button class="page-link">{{ paginaAtual }}</button>
            </li>

            <li class="page-item" v-show="consulta.possuiProximaPagina">
              <button class="page-link" @click="irParaProximaPagina()">
                {{ paginaAtual + 1 }}
              </button>
            </li>
            <!-- <h3>...</h3>-->
            <li
              class="page-item"
              v-show="paginaAtual < consulta.totalPaginas - 1"
            >
              <button class="page-link" @click="irParaUltimaPagina()">
                {{ consulta.totalPaginas }}
              </button>
            </li>

            <li class="page-item ml-5">
              <button
                class="page-link"
                @click="irParaProximaPagina()"
                :disabled="!consulta.possuiProximaPagina"
              >
                Próxima
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Busca",
  data() {
    return {
      termoBusca: "",
      paginaAtual: 1,
      itensPorPagina: 3,
      limiteCaracteresDescricao: 35,
      limiteCaracteresNome: 20,
      consulta: {
        resultados: []
      }
    };
  },
  methods: {
    realizarBusca() {
      this.$http
        .get(
          "https://localhost:5001/api/busca/" +
            this.termoBusca +
            "/" +
            this.paginaAtual +
            "/" +
            this.itensPorPagina
        )
        .then(res => res.json())
        .then(resultados => {
          this.consulta = resultados;
          console.log(this.consulta);
        });
    },
    irParaProximaPagina() {
      this.paginaAtual += 1;
      this.realizarBusca();
    },
    irParaPaginaAnterior() {
      this.paginaAtual -= 1;
      this.realizarBusca();
    },
    irParaPrimeiraPagina() {
      this.paginaAtual = 1;
      this.realizarBusca();
    },
    irParaUltimaPagina() {
      this.paginaAtual = this.consulta.totalPaginas;
      this.realizarBusca();
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.barraBusca {
  margin: 0 20px 0 20px;
  background-color: #e3f2fd;
}
</style>
