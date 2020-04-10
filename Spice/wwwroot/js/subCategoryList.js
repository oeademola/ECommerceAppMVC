function updateSubCategoryCollection() {
    var categorySelected = document.getElementById("ddlCategoryCollection").value;

    $list = $('#SubCategoryCollection');

    $.ajax({
        url: '/Admin/SubCategory/GetSubcategory/' + categorySelected,
        type: 'GET',
        dataType: 'text',
        success: function (data) {
            results = JSON.parse(data);
            $list.html('');
            $list.append(' <ul class= "list-group"> ');
            for (i in results) {
                $list.append('<li class = "list-group-item">' + results[i].text + '</li>');
            }
            $list.append('</ul>');
        }

    });
}

$(document).ready(function () {
    updateSubCategoryCollection();
});

$("#ddlCategoryCollection").on("change", function () {
    updateSubCategoryCollection();
});
