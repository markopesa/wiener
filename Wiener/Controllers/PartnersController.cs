using Microsoft.AspNetCore.Mvc;
using Wiener.Data.Interfaces;
using Wiener.Models.ViewModels;

namespace Wiener.Controllers
{
    public class PartnersController : Controller
    {
        private readonly IPartnerRepository _partnerRepository;
        private readonly IPolicyRepository _policyRepository;

        public PartnersController(
            IPartnerRepository partnerRepository,
            IPolicyRepository policyRepository
        )
        {
            _partnerRepository = partnerRepository;
            _policyRepository = policyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? newPartnerId)
        {
            try
            {
                var viewModel = new PartnerListViewModel
                {
                    NewlyAddedPartnerId = newPartnerId
                };

                await viewModel.LoadPartnersAsync(
                    _partnerRepository,
                    _policyRepository
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartnerDetailsModal(int id)
        {
            try
            {
                var viewModel = await PartnerDetailsViewModel.LoadAsync(
                    id,
                    _partnerRepository,
                    _policyRepository
                );

                if (viewModel == null)
                    return NotFound();

                return PartialView("_PartnerDetailsModal", viewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Greška prilikom dohvaćanja detalja partnera");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CreatePartnerViewModel();
                await viewModel.PrepareSelectListsAsync(_partnerRepository);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Greška prilikom učitavanja forme";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePartnerViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await model.PrepareSelectListsAsync(_partnerRepository);
                    return View(model);
                }

                var existingPartners = await _partnerRepository.GetAllAsync();
                if (existingPartners.Any(p => p.PartnerNumber == model.PartnerNumber))
                {
                    ModelState.AddModelError("PartnerNumber",
                        "Partner s ovim brojem već postoji");
                    await model.PrepareSelectListsAsync(_partnerRepository);
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.ExternalCode))
                {
                    if (existingPartners.Any(p => p.ExternalCode == model.ExternalCode))
                    {
                        ModelState.AddModelError("ExternalCode",
                            "Partner s ovim eksternim kodom već postoji");
                        await model.PrepareSelectListsAsync(_partnerRepository);
                        return View(model);
                    }
                }

                var newPartnerId = await model.SavePartnerAsync(_partnerRepository);

                TempData["SuccessMessage"] = "Partner je uspješno kreiran!";
                return RedirectToAction("Index", new { newPartnerId });
            }
            catch (Exception ex)
            {
                await model.PrepareSelectListsAsync(_partnerRepository);
                ModelState.AddModelError("",
                    "Greška prilikom spremanja partnera: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreatePolicyModal(int partnerId)
        {
            try
            {
                var viewModel = await CreatePolicyViewModel.PrepareAsync(
                    partnerId,
                    _partnerRepository
                );

                return PartialView("_CreatePolicyModal", viewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Greška prilikom učitavanja forme za policu");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePolicy(CreatePolicyViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                await model.SavePolicyAsync(_policyRepository);

                return Json(new
                {
                    success = true,
                    message = "Polica je uspješno dodana!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "Greška prilikom spremanja police" }
                });
            }
        }
    }
}