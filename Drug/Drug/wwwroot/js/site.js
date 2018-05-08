﻿$(document).ready(function () {
    var $registerRadios = $('input[name="ManufacturerType"]');
    if ($registerRadios.length) {
        showManufacturerView();
        $registerRadios.on('change', function (e) {
            showManufacturerView();
        })
    }

    var $registerRadios2 = $('input[name="SpecializationType"]');
    if ($registerRadios2.length) {
        showSpecializationView();
        $registerRadios2.on('change', function (e) {
            showSpecializationView();
        })
    }
});

function showManufacturerView() {
    var value = $('input[name="ManufacturerType"]:checked').first().val();
    if (value === "new") {
        $('#drugCreateManufacturer').show();
        $('#drugSelectManufacturer').hide();
    } else {
        $('#drugCreateManufacturer').hide();
        $('#drugSelectManufacturer').show();
    }
}

function showSpecializationView() {
    var value = $('input[name="SpecializationType"]:checked').first().val();
    if (value === "new") {
        $('#docCreateSpecialization').show();
        $('#docSelectSpecialization').hide();
    } else {
        $('#docCreateSpecialization').hide();
        $('#docSelectSpecialization').show();
    }
}
