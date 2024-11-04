$(document).ready(function () {
    $('input[type="file"].image-upload').on('change', function () {
        const file = this.files[0];

        const validTypes = ['image/jpeg', 'image/jpg', 'image/png'];

        if (file && validTypes.includes(file.type)) {
            const formData = new FormData();
            formData.append('file', file);

            const submitButton = $(this).closest('form').find('button[type="submit"]');

            const submitButtonText = submitButton.text();

            submitButton.prop('disabled', true).text('Uploading...');

            $.ajax({
                url: '/Images/UploadImage', // URL to upload endpoint
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function () {
                    // Optionally show a loader
                    submitButton.append(' <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');
                },
                success: function (response) {
                    const previewContainer = $(this).data('preview-container');
                    const hiddenInput = $(this).data('hidden-input');

                    $(hiddenInput).val(response.id);

                    if (previewContainer) {
                        const imagePreviewElement = $(previewContainer).find("#imagePreviewElement");

                        if (imagePreviewElement.length) {
                            // Update existing image source
                            imagePreviewElement.attr("src", response.thumbnail);
                        } else {
                            // Add new image element if not present
                            $(previewContainer).html('<img src="' + response.thumbnail + '" id="imagePreviewElement" alt="Image Preview" style="max-width: 200px;"/>');
                        }
                    }
                }.bind(this), // Bind 'this' to access the input element
                error: function (error) {
                    console.error('Error uploading image:', error);
                },
                complete: function () {
                    // Re-enable the button and remove loading state
                    submitButton.prop('disabled', false).text(submitButtonText);
                    submitButton.find('.spinner-border').remove();
                }
            });
        } else {
            alert('Please select a valid image file (JPEG or PNG).');
            $(this).val('');
        }
    });
});