namespace Highdmin.Models
{
    public interface IEmpresaEntity
    {
        int EmpresaId { get; set; }
        Empresa? Empresa { get; set; }
    }
}