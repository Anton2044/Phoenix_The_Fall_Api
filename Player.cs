using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Phoenix_The_Fall_Web_application.DataBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Phoenix_The_Fall_Web_application.Models 
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Currency { get; set; }
        public virtual ICollection<PlayerWeapon> PlayerWeapons { get; set; }

        public virtual ICollection<PlayerClass> PlayerClasses { get; set; } // Связь через PlayerClass

        public List<PlayerGranate> PlayerGranates { get; set; }
    }


    public class Weapon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(255)]
        public string PrefabPath { get; set; }

        [Required]
        [StringLength(50)]
        public string WeaponCategory { get; set; } // pistol, rifle etc

        [Required]
        [StringLength(50)]
        public string WeaponСlassRequirement { get; set; } // main, secondary, gadget etc.

        [Required]
        [StringLength(50)]
        public string ClassRestriction { get; set; } // "all" или название класса
        public int Cost { get; set; } 
    }

    public class PlayerWeapon
    {
        [Key] // Добавляем Key
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int WeaponId { get; set; }
        public bool IsUnlocked { get; set; } = false;

        // НОВЫЕ поля для статистики
        public int TotalShots { get; set; } // Кол-во выстрелов
        public int Kills { get; set; } // Кол-во убийств
        public int Headshots { get; set; } // Кол-во убийств
        public double TimeUsed { get; set; } // Время использования (в секундах)

        // Связи с Player и Weapon
        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [ForeignKey("WeaponId")]
        public virtual Weapon Weapon { get; set; }
    }

    public class Class
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PlayerClass
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ClassId { get; set; }
        public int ClassLevel { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public virtual ClassLoadout ClassLoadout { get; set; } // ClassLoadout для этого сочетания игрока и класса
    }
    public class ClassLoadout
    {
        [Key]
        public int PlayerClassId { get; set; }

        public int? MainWeaponId { get; set; }

        [ForeignKey("MainWeaponId")]
        public virtual PlayerWeapon MainWeapon { get; set; }

        public int? SecondaryWeaponId { get; set; }

        [ForeignKey("SecondaryWeaponId")]
        public virtual PlayerWeapon SecondaryWeapon { get; set; }

        public int? GadgetId { get; set; }

        [ForeignKey("GadgetId")]
        public virtual PlayerWeapon Gadget { get; set; }

        public int? GranateId { get; set; } // Новый столбец для гранаты, nullable

        // Навигационное свойство для гранаты (опционально)
        public virtual PlayerGranate Granate { get; set; }
    }
    public class Granate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrefabPath { get; set; }
        public string ClassRestriction { get; set; }
        public decimal Cost { get; set; }
    }

    public class PlayerGranate
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int WeaponId { get; set; } // Ссылка на Id в Granates
        public bool IsUnlocked { get; set; }
        public int TotalKills { get; set; }

        // Навигационное свойство (опционально, если нужно связать с Granate)
        public virtual Granate Granate { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } // Время начала
        public DateTime? EndTime { get; set; } // Время окончания (nullable)
        public float Team1Score { get; set; } // Очки первой команды
        public float Team2Score { get; set; } // Очки второй команды
        public int MapId { get; set; } // Связь с картой
        public Map Map { get; set; } // Навигационное свойство
    }
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; } // Название карты
        public string Description { get; set; } // Описание
        public int SizeX { get; set; } // Размер по X
        public int SizeY { get; set; } // Размер по Y
        public int SectorCount { get; set; } // Кол-во секторов
    }
}
