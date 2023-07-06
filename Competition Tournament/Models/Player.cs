using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Models;

[Table("Player")]
public partial class Player
{
    [Key]
    public int Id { get; set; }

    [Column("Last_Name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? LastName { get; set; }

    [Column("First_Name")]
    [StringLength(255)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    public int? Age { get; set; }

    [Column("Team_Id")]
    public int? TeamId { get; set; }

    [ForeignKey("TeamId")]
    [InverseProperty("Players")]
    public virtual Team? Team { get; set; }
}
