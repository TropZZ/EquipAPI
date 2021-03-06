﻿using equip.api.Business.Entities;
using equip.api.Business.Repositories;
using equip.api.Models.Equips;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace equip.api.Controllers
{
    [Route("api/v1/equips")]
    [ApiController]
    [Authorize]
    public class EquipsController : ControllerBase
    {
        private readonly IEquipRepository _equipRepository;

        public EquipsController(IEquipRepository equipRepository)
        {
            _equipRepository = equipRepository;
        }

        /// <summary>
        /// Este serviço permite cadastrar equipamentos para o usuario autenticado
        /// </summary>
        /// <param name="equipsViewModelInput"></param>
        /// <returns>Retorna o status 201 e dados do equipamento do usuario</returns>
        [SwaggerResponse(statusCode: 201, description: "Sucesso ao cadastrar um equipamento")]
        [SwaggerResponse(statusCode: 401, description: "Não Autorizado")]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Post(EquipsViewModelInput equipsViewModelInput)
        {
            Equip equip = new Equip();

            equip.Name = equipsViewModelInput.Name;
            equip.Damage = equipsViewModelInput.Damage;
            var userCode = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            equip.UserCode = userCode;
            _equipRepository.Add(equip);
            _equipRepository.Commit();

            return Created("", equipsViewModelInput);
        }

        /// <summary>
        /// Este serviço permite obter todos os equipamentos ativos do usuario
        /// </summary>
        /// <returns>Retorna status ok e dados do equipamento do usuario</returns>
        [SwaggerResponse(statusCode: 201, description: "Sucesso ao obter os equipamentos")]
        [SwaggerResponse(statusCode: 401, description: "Não Autorizado")]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var userCode = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            var equip = _equipRepository.GetByUser(userCode)
                .Select(s => new EquipViewModelOutput()
                {
                    Name = s.Name,
                    Damage = s.Damage,
                    Login = s.User.Login
                }); 

            return Ok(equip);
        }
    }
}
