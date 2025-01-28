import { AxiosError } from "axios";
import Swal from "sweetalert2";

interface SPARQLQuerryError {
  error: string;
  validationError?: string;
  details?: string;
}

const isSPARQLQuerryError = (data: any): data is SPARQLQuerryError => {
  return (
    typeof data === "object" &&
    data !== null &&
    typeof data.error === "string" &&
    (typeof data.validationError === "string" ||
      data.validationError === undefined)
  );
};

export const handleRequestError = (error: AxiosError) => {
  const responseData = error?.response?.data;

  if (isSPARQLQuerryError(responseData)) {
    Swal.fire({
      title: `${responseData.error}`,
      text: `${responseData.validationError}`,
      icon: "error",
      confirmButtonText: "Ok",
      confirmButtonColor: "black",
    });
  } else {
    Swal.fire({
      title: `Error`,
      text: `${error}`,
      icon: "error",
      confirmButtonText: "Ok",
      confirmButtonColor: "black",
    });
  }

  console.error(error);
};
