namespace backend;

public class MicroOndasDto
{
    public string Status { get; set; }
    public int TempoRestante { get; set; }
    public string Alimento { get; set; }
}

public class MicroOndas
{
    private int tempoMax = 120;
    private DateTime horaInicio;
    private DateTime horaPausa;
    // private bool pausado;
    private int potencia;
    private int tempo;
    private string status;

    private string aquecimento;

    private string alimento;

    public MicroOndas()
    {
        this.Inicializar();
    }

    public DateTime HoraInicio { get => horaInicio; }
    // public DateTime HoraPausa { get => horaPausa; }
    public int Potencia { get => potencia; }

    private void Inicializar() {
        this.horaInicio = DateTime.MaxValue;
        this.status = "INATIVO";
        this.alimento = "";
        this.aquecimento = "";
    }

    private void MontarStringAquecimento () {

        string umSeg = "";
        for (int i=0; i<this.potencia; i++)
            umSeg += ".";
        umSeg += " ";

        this.aquecimento = "";
        for (int i=0; i<this.tempoMax; i++)
            this.aquecimento += umSeg;

    }

    public void Iniciar(int potencia, int tempo) {

        if (this.status == "INATIVO") {
            this.horaInicio = DateTime.Now;
            this.potencia = potencia;
            this.tempo = tempo;
            this.MontarStringAquecimento();
            this.alimento = "";
            this.status = "ATIVO";
        } else {
            this.tempo = tempo + 30;
        }

    }

    public void PausarOuCancelar() {
        if (this.status == "ATIVO") {
            this.horaPausa = DateTime.Now;
            this.status = "PAUSADO";
        } else if (this.status == "PAUSADO") {
            this.Inicializar();
        }
    }

    public MicroOndasDto Status() {

        DateTime agora = DateTime.Now;
        int tempoRestante;

        if (this.horaInicio == DateTime.MaxValue) {

            this.status = "INATIVO";
            tempoRestante = this.tempo;

        } else if (agora >= horaInicio.AddSeconds(this.tempo)) {

            if (this.status == "ATIVO") {
                this.alimento += " Aquecimento concluído";
                this.status = "INATIVO";
            }

            tempoRestante = this.tempo;

        } else {

            int tempoDecorrido = Convert.ToInt32(agora.Subtract(this.horaInicio).TotalSeconds);
            tempoRestante = this.tempo - tempoDecorrido;
            this.alimento = this.aquecimento.Substring(0, (potencia+1) * tempoDecorrido);
        }

        return new MicroOndasDto() {
            Status = this.status,
            TempoRestante = tempoRestante,
            Alimento = this.alimento
        };
    }
}