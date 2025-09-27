using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Phoenix_The_Fall_Web_application.DataBase; 
using Phoenix_The_Fall_Web_application.Models; 
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Phoenix_The_Fall_Web_application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {
        private readonly YourDbContext _context;

        public PlayerController(YourDbContext context)
        {
            _context = context;
        }

        // GET: api/player
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        // POST: api/player
        [HttpPost]
        public async Task<ActionResult<Player>> AddPlayer(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPlayer", new { id = player.Id }, player);
        }

        // GET: api/Player/GetOrCreatePlayer/{username}
        [HttpGet("GetOrCreatePlayer/{username}")]
        public async Task<ActionResult<object>> GetOrCreatePlayer([FromRoute] string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length > 50)
            {
                return BadRequest("Недопустимое имя пользователя.");
            }

            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.Username == username);

            bool isNewPlayer = player == null;
            if (isNewPlayer)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    player = new Player
                    {
                        Username = username,
                        Kills = 0,
                        Deaths = 0,
                        Experience = 0,
                        Level = 1,
                        Currency = 0
                    };
                    _context.Players.Add(player);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Создан игрок: Id={player.Id}, Username={player.Username}");

                    // Создание PlayerClasses
                    var classes = await _context.Classes.ToListAsync();
                    if (!classes.Any())
                    {
                        Console.WriteLine("Предупреждение: таблица Classes пуста. PlayerClasses не созданы.");
                    }
                    else
                    {
                        var existingPlayerClasses = await _context.PlayerClasses
                            .Where(pc => pc.PlayerId == player.Id)
                            .Select(pc => pc.ClassId)
                            .ToListAsync();

                        foreach (var classEntity in classes)
                        {
                            if (!existingPlayerClasses.Contains(classEntity.Id))
                            {
                                var playerClass = new PlayerClass
                                {
                                    PlayerId = player.Id,
                                    ClassId = classEntity.Id,
                                    ClassLevel = 1
                                };
                                _context.PlayerClasses.Add(playerClass);
                                Console.WriteLine($"Создаётся PlayerClass: PlayerId={playerClass.PlayerId}, ClassId={playerClass.ClassId}, Id={playerClass.Id}");
                            }
                        }
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Создано {classes.Count - existingPlayerClasses.Count} записей в PlayerClasses для игрока {player.Id}");
                    }

                    // Создание PlayerWeapons
                    var weapons = await _context.Weapons.ToListAsync();
                    var existingPlayerWeapons = await _context.PlayerWeapons
                        .Where(pw => pw.PlayerId == player.Id)
                        .Select(pw => pw.WeaponId)
                        .ToListAsync();

                    var newPlayerWeapons = new List<PlayerWeapon>();
                    if (!weapons.Any())
                    {
                        Console.WriteLine("Предупреждение: таблица Weapons пуста. PlayerWeapons не созданы.");
                    }
                    else
                    {
                        foreach (var weapon in weapons)
                        {
                            if (!existingPlayerWeapons.Contains(weapon.Id))
                            {
                                var playerWeapon = new PlayerWeapon
                                {
                                    PlayerId = player.Id,
                                    WeaponId = weapon.Id,
                                    IsUnlocked = true,
                                    TimeUsed = 0.0,
                                    Kills = 0,
                                    TotalShots = 0,
                                    Headshots = 0
                                };
                                Console.WriteLine($"Создаётся PlayerWeapon: PlayerId={playerWeapon.PlayerId}, WeaponId={playerWeapon.WeaponId}, Id={playerWeapon.Id}, IsUnlocked={playerWeapon.IsUnlocked}, TimeUsed={playerWeapon.TimeUsed}, Kills={playerWeapon.Kills}, TotalShots={playerWeapon.TotalShots}, Headshots={playerWeapon.Headshots}");
                                newPlayerWeapons.Add(playerWeapon);
                            }
                        }

                        if (newPlayerWeapons.Any())
                        {
                            _context.PlayerWeapons.AddRange(newPlayerWeapons);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Создано {newPlayerWeapons.Count} записей в PlayerWeapons для игрока {player.Id}");
                        }
                    }

                    // Создание PlayerGranates
                    var granates = await _context.Granates.ToListAsync();
                    var existingPlayerGranates = await _context.PlayerGranates
                        .Where(pg => pg.PlayerId == player.Id)
                        .Select(pg => pg.WeaponId)
                        .ToListAsync();

                    var newPlayerGranates = new List<PlayerGranate>();
                    if (!granates.Any())
                    {
                        Console.WriteLine("Предупреждение: таблица Granates пуста. PlayerGranates не созданы.");
                    }
                    else
                    {
                        foreach (var granate in granates)
                        {
                            if (!existingPlayerGranates.Contains(granate.Id))
                            {
                                var playerGranate = new PlayerGranate
                                {
                                    PlayerId = player.Id,
                                    WeaponId = granate.Id,
                                    IsUnlocked = true,
                                    TotalKills = 0
                                };
                                Console.WriteLine($"Создаётся PlayerGranate: PlayerId={playerGranate.PlayerId}, WeaponId={playerGranate.WeaponId}, Id={playerGranate.Id}, IsUnlocked={playerGranate.IsUnlocked}, TotalKills={playerGranate.TotalKills}");
                                newPlayerGranates.Add(playerGranate);
                            }
                        }

                        if (newPlayerGranates.Any())
                        {
                            _context.PlayerGranates.AddRange(newPlayerGranates);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Создано {newPlayerGranates.Count} записей в PlayerGranates для игрока {player.Id}");
                        }
                    }

                    // Создание ClassLoadout
                    var playerClassesForLoadout = await _context.PlayerClasses
                        .Where(pc => pc.PlayerId == player.Id)
                        .ToListAsync();

                    if (!playerClassesForLoadout.Any())
                    {
                        Console.WriteLine("Предупреждение: PlayerClasses пусты. ClassLoadout не созданы.");
                    }
                    else
                    {
                        var existingClassLoadouts = await _context.ClassLoadouts
                            .Where(cl => cl.PlayerClassId != null && playerClassesForLoadout.Select(pc => pc.Id).Contains(cl.PlayerClassId))
                            .Select(cl => cl.PlayerClassId)
                            .ToListAsync();

                        var newClassLoadouts = new List<ClassLoadout>();
                        if (newPlayerWeapons.Any() && newPlayerGranates.Any())
                        {
                            var defaultWeapon = newPlayerWeapons.First();
                            var defaultGranate = newPlayerGranates.First(); // Берём первую гранату

                            foreach (var playerClass in playerClassesForLoadout)
                            {
                                if (!existingClassLoadouts.Contains(playerClass.Id))
                                {
                                    var classLoadout = new ClassLoadout
                                    {
                                        PlayerClassId = playerClass.Id,
                                        MainWeaponId = defaultWeapon.Id,
                                        SecondaryWeaponId = defaultWeapon.Id,
                                        GadgetId = defaultWeapon.Id,
                                        GranateId = defaultGranate.Id // Устанавливаем первую гранату
                                    };
                                    Console.WriteLine($"Создаётся ClassLoadout: PlayerClassId={classLoadout.PlayerClassId}, MainWeaponId={classLoadout.MainWeaponId}, SecondaryWeaponId={classLoadout.SecondaryWeaponId}, GadgetId={classLoadout.GadgetId}, GranateId={classLoadout.GranateId}");
                                    newClassLoadouts.Add(classLoadout);
                                }
                            }
                        }

                        if (newClassLoadouts.Any())
                        {
                            _context.ClassLoadouts.AddRange(newClassLoadouts);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Создано {newClassLoadouts.Count} записей в ClassLoadout для игрока {player.Id}");
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Ошибка при создании игрока: {ex.Message}");
                    return StatusCode(500, "Ошибка при создании игрока.");
                }
            }

            var playerClasses = await _context.PlayerClasses
                .Where(pc => pc.PlayerId == player.Id)
                .Select(pc => new
                {
                    pc.Id,
                    pc.ClassId,
                    pc.ClassLevel,
                    Class = new
                    {
                        pc.Class.Id,
                        pc.Class.Name,
                        pc.Class.Description
                    },
                    ClassLoadout = pc.ClassLoadout != null ? new
                    {
                        pc.ClassLoadout.MainWeaponId,
                        pc.ClassLoadout.SecondaryWeaponId,
                        pc.ClassLoadout.GadgetId,
                        pc.ClassLoadout.GranateId
                    } : null
                })
                .ToListAsync();

            var playerGranates = await _context.PlayerGranates
                .Where(pg => pg.PlayerId == player.Id)
                .Select(pg => new
                {
                    pg.Id,
                    pg.PlayerId,
                    pg.WeaponId,
                    pg.IsUnlocked,
                    pg.TotalKills,
                    Granate = new
                    {
                        pg.Granate.Id,
                        pg.Granate.Name,
                        pg.Granate.Description,
                        pg.Granate.PrefabPath,
                        pg.Granate.ClassRestriction,
                        pg.Granate.Cost
                    }
                })
                .ToListAsync();

            var playerResponse = new
            {
                Player = new
                {
                    player.Id,
                    player.Username,
                    player.Kills,
                    player.Deaths,
                    player.Experience,
                    player.Level,
                    player.Currency
                },
                PlayerClasses = playerClasses,
                PlayerGranates = playerGranates,
                IsNewPlayer = isNewPlayer
            };

            return Ok(playerResponse);
        }

        // GET: api/Player/GetPlayerClasses/{playerId}
        [HttpGet("GetPlayerClasses/{playerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPlayerClasses(int playerId)
        {
            var playerClasses = await _context.PlayerClasses
                .Include(pc => pc.ClassLoadout)
                    .ThenInclude(cl => cl.MainWeapon)
                        .ThenInclude(pw => pw.Weapon)
                .Include(pc => pc.ClassLoadout)
                    .ThenInclude(cl => cl.SecondaryWeapon)
                        .ThenInclude(pw => pw.Weapon)
                .Include(pc => pc.ClassLoadout)
                    .ThenInclude(cl => cl.Gadget)
                        .ThenInclude(pw => pw.Weapon)
                .Include(pc => pc.ClassLoadout)
                    .ThenInclude(cl => cl.Granate)
                        .ThenInclude(pg => pg.Granate) // Включаем Granate через PlayerGranate
                .Include(pc => pc.Class)
                .Where(pc => pc.PlayerId == playerId)
                .ToListAsync();

            if (playerClasses == null || playerClasses.Count == 0)
            {
                return NotFound("У игрока нет классов.");
            }

            var response = playerClasses.Select(playerClass => new
            {
                Id = playerClass.Id,
                ClassId = playerClass.ClassId,
                ClassLevel = playerClass.ClassLevel,
                Class = new
                {
                    playerClass.Class.Id,
                    playerClass.Class.Name,
                    playerClass.Class.Description
                },
                ClassLoadout = playerClass.ClassLoadout != null ? new
                {
                    MainWeapon = playerClass.ClassLoadout.MainWeapon != null ? new
                    {
                        playerClass.ClassLoadout.MainWeapon.Id,
                        playerClass.ClassLoadout.MainWeapon.PlayerId,
                        playerClass.ClassLoadout.MainWeapon.WeaponId,
                        playerClass.ClassLoadout.MainWeapon.IsUnlocked,
                        playerClass.ClassLoadout.MainWeapon.TimeUsed,
                        playerClass.ClassLoadout.MainWeapon.Kills,
                        playerClass.ClassLoadout.MainWeapon.TotalShots,
                        playerClass.ClassLoadout.MainWeapon.Headshots,
                        Weapon = new
                        {
                            playerClass.ClassLoadout.MainWeapon.Weapon.Id,
                            playerClass.ClassLoadout.MainWeapon.Weapon.Name,
                            playerClass.ClassLoadout.MainWeapon.Weapon.Description,
                            playerClass.ClassLoadout.MainWeapon.Weapon.PrefabPath,
                            playerClass.ClassLoadout.MainWeapon.Weapon.WeaponCategory,
                            playerClass.ClassLoadout.MainWeapon.Weapon.WeaponСlassRequirement,
                            playerClass.ClassLoadout.MainWeapon.Weapon.ClassRestriction,
                            playerClass.ClassLoadout.MainWeapon.Weapon.Cost
                        }
                    } : null,
                    SecondaryWeapon = playerClass.ClassLoadout.SecondaryWeapon != null ? new
                    {
                        playerClass.ClassLoadout.SecondaryWeapon.Id,
                        playerClass.ClassLoadout.SecondaryWeapon.PlayerId,
                        playerClass.ClassLoadout.SecondaryWeapon.WeaponId,
                        playerClass.ClassLoadout.SecondaryWeapon.IsUnlocked,
                        playerClass.ClassLoadout.SecondaryWeapon.TimeUsed,
                        playerClass.ClassLoadout.SecondaryWeapon.Kills,
                        playerClass.ClassLoadout.SecondaryWeapon.TotalShots,
                        playerClass.ClassLoadout.SecondaryWeapon.Headshots,
                        Weapon = new
                        {
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.Id,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.Name,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.Description,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.PrefabPath,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.WeaponCategory,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.WeaponСlassRequirement,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.ClassRestriction,
                            playerClass.ClassLoadout.SecondaryWeapon.Weapon.Cost
                        }
                    } : null,
                    Gadget = playerClass.ClassLoadout.Gadget != null ? new
                    {
                        playerClass.ClassLoadout.Gadget.Id,
                        playerClass.ClassLoadout.Gadget.PlayerId,
                        playerClass.ClassLoadout.Gadget.WeaponId,
                        playerClass.ClassLoadout.Gadget.IsUnlocked,
                        playerClass.ClassLoadout.Gadget.TimeUsed,
                        playerClass.ClassLoadout.Gadget.Kills,
                        playerClass.ClassLoadout.Gadget.TotalShots,
                        playerClass.ClassLoadout.Gadget.Headshots,
                        Weapon = new
                        {
                            playerClass.ClassLoadout.Gadget.Weapon.Id,
                            playerClass.ClassLoadout.Gadget.Weapon.Name,
                            playerClass.ClassLoadout.Gadget.Weapon.Description,
                            playerClass.ClassLoadout.Gadget.Weapon.PrefabPath,
                            playerClass.ClassLoadout.Gadget.Weapon.WeaponCategory,
                            playerClass.ClassLoadout.Gadget.Weapon.WeaponСlassRequirement,
                            playerClass.ClassLoadout.Gadget.Weapon.ClassRestriction,
                            playerClass.ClassLoadout.Gadget.Weapon.Cost
                        }
                    } : null,
                    Granate = playerClass.ClassLoadout.Granate != null ? new
                    {
                        playerClass.ClassLoadout.Granate.Id,
                        playerClass.ClassLoadout.Granate.PlayerId,
                        playerClass.ClassLoadout.Granate.WeaponId,
                        playerClass.ClassLoadout.Granate.IsUnlocked,
                        playerClass.ClassLoadout.Granate.TotalKills,
                        Granate = new
                        {
                            playerClass.ClassLoadout.Granate.Granate.Id,
                            playerClass.ClassLoadout.Granate.Granate.Name,
                            playerClass.ClassLoadout.Granate.Granate.Description,
                            playerClass.ClassLoadout.Granate.Granate.PrefabPath,
                            playerClass.ClassLoadout.Granate.Granate.ClassRestriction,
                            playerClass.ClassLoadout.Granate.Granate.Cost
                        }
                    } : null
                } : null
            }).ToList();

            return Ok(response);
        }

        [HttpGet("update/{id}")]
        public async Task<ActionResult<Player>> UpdatePlayer(int id, [FromQuery] string username, [FromQuery] int kills, [FromQuery] int deaths, [FromQuery] int experience)
        {
            Console.WriteLine($"Получен GET: Обновление игрока, Id={id}, Username={username}");
            var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
            if (player == null)
            {
                Console.WriteLine("Игрок не найден");
                return NotFound();
            }

            player.Username = username;
            player.Kills = kills;
            player.Deaths = deaths;
            player.Experience = experience;
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            Console.WriteLine("Данные игрока обновлены");

            return Ok(player);
        }

        [HttpGet("weapons/{playerId}")]
        public async Task<ActionResult<IEnumerable<PlayerWeapon>>> GetPlayerWeapons(int playerId)
        {
            var playerWeapons = await _context.PlayerWeapons
                .Include(pw => pw.Weapon)
                .Where(pw => pw.PlayerId == playerId)
                .ToListAsync();

            return Ok(playerWeapons);
        }

        [HttpGet("weapon/{playerId}/{weaponName}")]
        public async Task<ActionResult<PlayerWeapon>> GetOrUpdateWeapon(int playerId, string weaponName, [FromQuery] int totalShots = 0, [FromQuery] int totalKills = 0, [FromQuery] double timeUsed = 0)
        {
            Console.WriteLine($"Получен GET: PlayerId={playerId}, WeaponName={weaponName}");

            var weapon = await _context.Weapons.FirstOrDefaultAsync(w => w.Name == weaponName);
            if (weapon == null)
            {
                Console.WriteLine("Оружие не найдено");
                return NotFound("Оружие не найдено");
            }

            var existingPlayerWeapon = await _context.PlayerWeapons
                .FirstOrDefaultAsync(pw => pw.PlayerId == playerId && pw.WeaponId == weapon.Id);

            if (existingPlayerWeapon == null)
            {
                Console.WriteLine("Создаём новый PlayerWeapon");
                var playerWeapon = new PlayerWeapon
                {
                    PlayerId = playerId,
                    WeaponId = weapon.Id,
                    TotalShots = totalShots,
                    Kills = totalKills,
                    TimeUsed = timeUsed,
                    IsUnlocked = true 
                };
                _context.PlayerWeapons.Add(playerWeapon);
                existingPlayerWeapon = playerWeapon;
            }
            else
            {
                Console.WriteLine("Обновляем существующий PlayerWeapon");
                existingPlayerWeapon.TotalShots = totalShots;
                existingPlayerWeapon.Kills = totalKills;
                existingPlayerWeapon.TimeUsed = timeUsed;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine("Данные PlayerWeapon возвращены/сохранены");
            return Ok(existingPlayerWeapon);
        }

        // GET: api/player/match
        [HttpGet("match")]
        public async Task<ActionResult<Match>> CreateMatch([FromQuery] DateTime startTime, [FromQuery] int team1Score, [FromQuery] int team2Score, [FromQuery] int mapId)
        {
            Console.WriteLine($"Получен GET: Создание матча, StartTime={startTime}, MapId={mapId}");
            var match = new Match
            {
                StartTime = startTime,
                Team1Score = team1Score,
                Team2Score = team2Score,
                MapId = mapId
            };
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            Console.WriteLine("Матч создан");
            return Ok(match);
        }

        // GET: api/player/match/{matchId}
        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<Match>> GetMatch(int matchId)
        {
            var match = await _context.Matches.Include(m => m.Map).FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        // GET: api/player/match/update/{matchId}
        [HttpGet("match/update/{matchId}")]
        public async Task<ActionResult<Match>> UpdateMatch(int matchId, [FromQuery] DateTime? endTime, [FromQuery] int team1Score, [FromQuery] int team2Score, [FromQuery] int mapId)
        {
            Console.WriteLine($"Получен GET: Обновление матча, MatchId={matchId}");
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
            {
                return NotFound();
            }
            match.EndTime = endTime;
            match.Team1Score = team1Score;
            match.Team2Score = team2Score;
            match.MapId = mapId;
            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
            Console.WriteLine("Матч обновлён");
            return Ok(match);
        }

        // GET: api/player/maps
        [HttpGet("maps")]
        public async Task<ActionResult<IEnumerable<Map>>> GetMaps()
        {
            return await _context.Maps.ToListAsync();
        }

        // GET: api/player/map
        [HttpGet("map")]
        public async Task<ActionResult<Map>> AddMap([FromQuery] string name, [FromQuery] string description, [FromQuery] int sizeX, [FromQuery] int sizeY, [FromQuery] int sectorCount)
        {
            Console.WriteLine($"Получен GET: Добавление карты, Name={name}");
            var map = new Map
            {
                Name = name,
                Description = description,
                SizeX = sizeX,
                SizeY = sizeY,
                SectorCount = sectorCount
            };
            _context.Maps.Add(map);
            await _context.SaveChangesAsync();
            Console.WriteLine("Карта добавлена");
            return Ok(map);
        }

        // GET: api/player/map/{mapId}
        [HttpGet("map/{mapId}")]
        public async Task<ActionResult<Map>> GetMap(int mapId)
        {
            var map = await _context.Maps.FirstOrDefaultAsync(m => m.Id == mapId);
            if (map == null)
            {
                return NotFound();
            }
            return Ok(map);
        }

        [HttpGet("GetAvailableWeapons/{playerId}/{classId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableWeapons(int playerId, int classId)
        {
            var classEntity = await _context.Classes.FindAsync(classId);
            if (classEntity == null)
            {
                return NotFound("Класс не найден.");
            }

            var playerWeapons = await _context.PlayerWeapons
                .Include(pw => pw.Weapon)
                .Where(pw => pw.PlayerId == playerId &&
                             (pw.Weapon.ClassRestriction == "all" || pw.Weapon.ClassRestriction == classEntity.Name))
                .ToListAsync();

            var response = playerWeapons.Select(pw => new
            {
                Id = pw.Id,
                PlayerId = pw.PlayerId,
                WeaponId = pw.WeaponId,
                IsUnlocked = pw.IsUnlocked,
                TimeUsed = pw.TimeUsed,
                Kills = pw.Kills,
                TotalShots = pw.TotalShots,
                Headshots = pw.Headshots,
                Weapon = new
                {
                    pw.Weapon.Id,
                    pw.Weapon.Name,
                    pw.Weapon.Description,
                    pw.Weapon.PrefabPath,
                    pw.Weapon.WeaponCategory,
                    pw.Weapon.WeaponСlassRequirement,
                    pw.Weapon.ClassRestriction,
                    pw.Weapon.Cost
                }
            }).ToList();
            Debug.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%" + JsonConvert.SerializeObject(response));
            return Ok(response);
        }
        // GET: api/Player/GetAvailableGranates/{playerId}/{classId}
        [HttpGet("GetAvailableGranates/{playerId}/{classId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAvailableGranates(int playerId, int classId)
        {
            var classEntity = await _context.Classes.FindAsync(classId);
            if (classEntity == null)
            {
                return NotFound("Класс не найден.");
            }

            var playerGranates = await _context.PlayerGranates
                .Include(pg => pg.Granate)
                .Where(pg => pg.PlayerId == playerId && pg.IsUnlocked)
                .Select(pg => pg.Granate)
                .Where(g => g.ClassRestriction == "all" || g.ClassRestriction == classEntity.Name)
                .ToListAsync();

            var response = playerGranates.Select(g => new
            {
                g.Id,
                g.Name,
                g.Description,
                g.PrefabPath,
                g.ClassRestriction,
                g.Cost
            }).ToList();

            return Ok(response);
        }

        // GET: api/Player/GetPlayerGranates/{playerId}
        [HttpGet("GetPlayerGranates/{playerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPlayerGranates(int playerId)
        {
            var playerGranates = await _context.PlayerGranates
                .Include(pg => pg.Granate)
                .Where(pg => pg.PlayerId == playerId)
                .ToListAsync();

            var response = playerGranates.Select(pg => new
            {
                pg.Id,
                pg.PlayerId,
                pg.WeaponId,
                pg.IsUnlocked,
                pg.TotalKills,
                Granate = new
                {
                    pg.Granate.Id,
                    pg.Granate.Name,
                    pg.Granate.Description,
                    pg.Granate.PrefabPath,
                    pg.Granate.ClassRestriction,
                    pg.Granate.Cost
                }
            }).ToList();

            return Ok(response);
        }

        // GET: api/Player/GetClassLoadoutsForPlayer/{playerId}
        [HttpGet("GetClassLoadoutsForPlayer/{playerId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetClassLoadoutsForPlayer(int playerId)
        {
            var classLoadouts = await _context.ClassLoadouts
                .Include(cl => cl.MainWeapon)
                    .ThenInclude(mw => mw.Weapon)
                .Include(cl => cl.SecondaryWeapon)
                    .ThenInclude(sw => sw.Weapon)
                .Include(cl => cl.Gadget)
                    .ThenInclude(g => g.Weapon)
                .Include(cl => cl.Granate)
                    .ThenInclude(pg => pg.Granate)
                .Where(cl => cl.PlayerClassId == playerId)
                .ToListAsync();

            var response = classLoadouts.Select(cl => new
            {
                cl.PlayerClassId,
                MainWeapon = cl.MainWeapon != null ? new
                {
                    cl.MainWeapon.Id,
                    cl.MainWeapon.PlayerId,
                    cl.MainWeapon.WeaponId,
                    cl.MainWeapon.IsUnlocked,
                    cl.MainWeapon.TimeUsed,
                    cl.MainWeapon.Kills,
                    cl.MainWeapon.TotalShots,
                    cl.MainWeapon.Headshots,
                    Weapon = new
                    {
                        cl.MainWeapon.Weapon.Id,
                        cl.MainWeapon.Weapon.Name,
                        cl.MainWeapon.Weapon.Description,
                        cl.MainWeapon.Weapon.PrefabPath,
                        cl.MainWeapon.Weapon.WeaponCategory,
                        cl.MainWeapon.Weapon.WeaponСlassRequirement,
                        cl.MainWeapon.Weapon.ClassRestriction,
                        cl.MainWeapon.Weapon.Cost
                    }
                } : null,
                SecondaryWeapon = cl.SecondaryWeapon != null ? new
                {
                    cl.SecondaryWeapon.Id,
                    cl.SecondaryWeapon.PlayerId,
                    cl.SecondaryWeapon.WeaponId,
                    cl.SecondaryWeapon.IsUnlocked,
                    cl.SecondaryWeapon.TimeUsed,
                    cl.SecondaryWeapon.Kills,
                    cl.SecondaryWeapon.TotalShots,
                    cl.SecondaryWeapon.Headshots,
                    Weapon = new
                    {
                        cl.SecondaryWeapon.Weapon.Id,
                        cl.SecondaryWeapon.Weapon.Name,
                        cl.SecondaryWeapon.Weapon.Description,
                        cl.SecondaryWeapon.Weapon.PrefabPath,
                        cl.SecondaryWeapon.Weapon.WeaponCategory,
                        cl.SecondaryWeapon.Weapon.WeaponСlassRequirement,
                        cl.SecondaryWeapon.Weapon.ClassRestriction,
                        cl.SecondaryWeapon.Weapon.Cost
                    }
                } : null,
                Gadget = cl.Gadget != null ? new
                {
                    cl.Gadget.Id,
                    cl.Gadget.PlayerId,
                    cl.Gadget.WeaponId,
                    cl.Gadget.IsUnlocked,
                    cl.Gadget.TimeUsed,
                    cl.Gadget.Kills,
                    cl.Gadget.TotalShots,
                    cl.Gadget.Headshots,
                    Weapon = new
                    {
                        cl.Gadget.Weapon.Id,
                        cl.Gadget.Weapon.Name,
                        cl.Gadget.Weapon.Description,
                        cl.Gadget.Weapon.PrefabPath,
                        cl.Gadget.Weapon.WeaponCategory,
                        cl.Gadget.Weapon.WeaponСlassRequirement,
                        cl.Gadget.Weapon.ClassRestriction,
                        cl.Gadget.Weapon.Cost
                    }
                } : null,
                Granate = cl.Granate != null ? new
                {
                    cl.Granate.Id,
                    cl.Granate.PlayerId,
                    cl.Granate.WeaponId,
                    cl.Granate.IsUnlocked,
                    cl.Granate.TotalKills,
                    GranateDetails = new
                    {
                        cl.Granate.Granate.Id,
                        cl.Granate.Granate.Name,
                        cl.Granate.Granate.Description,
                        cl.Granate.Granate.PrefabPath,
                        cl.Granate.Granate.ClassRestriction,
                        cl.Granate.Granate.Cost
                    }
                } : null
            }).ToList();

            return Ok(response);
        }

        // POST: api/Player/SaveLoadout/{classId}/{mainWeaponId}/{secondaryWeaponId}/{gadgetId}/{granateId}
        [HttpPost("SaveLoadout/{classId}/{mainWeaponId}/{secondaryWeaponId}/{gadgetId}/{granateId}")]
        public async Task<ActionResult> SaveLoadout(int classId, int mainWeaponId, int secondaryWeaponId, int gadgetId, int? granateId)
        {
            var playerClass = await _context.PlayerClasses
                .Include(pc => pc.ClassLoadout)
                .FirstOrDefaultAsync(pc => pc.Id == classId);

            if (playerClass == null)
            {
                return NotFound("PlayerClass не найден.");
            }

            var mainWeapon = await _context.PlayerWeapons.FindAsync(mainWeaponId);
            if (mainWeapon == null)
            {
                return NotFound("MainWeapon не найден.");
            }

            var secondaryWeapon = await _context.PlayerWeapons.FindAsync(secondaryWeaponId);
            if (secondaryWeapon == null)
            {
                return NotFound("SecondaryWeapon не найден.");
            }

            var gadget = await _context.PlayerWeapons.FindAsync(gadgetId);
            if (gadget == null)
            {
                return NotFound("Gadget не найден.");
            }

            PlayerGranate granate = null;
            if (granateId.HasValue)
            {
                granate = await _context.PlayerGranates
                    .Include(pg => pg.Granate)
                    .FirstOrDefaultAsync(pg => pg.Id == granateId.Value && pg.PlayerId == playerClass.PlayerId);
                if (granate == null)
                {
                    return NotFound("Granate не найден или не принадлежит игроку.");
                }
            }

            if (playerClass.ClassLoadout == null)
            {
                playerClass.ClassLoadout = new ClassLoadout
                {
                    PlayerClassId = playerClass.Id
                };
                _context.ClassLoadouts.Add(playerClass.ClassLoadout);
            }

            playerClass.ClassLoadout.MainWeaponId = mainWeaponId;
            playerClass.ClassLoadout.SecondaryWeaponId = secondaryWeaponId;
            playerClass.ClassLoadout.GadgetId = gadgetId;
            playerClass.ClassLoadout.GranateId = granateId;

            await _context.SaveChangesAsync();
            return Ok("ClassLoadout успешно сохранён.");
        }
    
    }
}

