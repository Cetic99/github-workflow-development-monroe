/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconDeliveryTruck = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M2 7C2 5.34315 3.34315 4 5 4H10C11.6569 4 13 5.34315 13 7V16C13 17.1046 12.1046 18 11 18H8V16H11V7C11 6.44772 10.5523 6 10 6H5C4.44772 6 4 6.44772 4 7V15C4 15.5523 4.44772 16 5 16H6V18H5C3.34315 18 2 16.6569 2 15V7Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M11 7H16.2251C16.9729 7 17.6938 7.27929 18.2464 7.78313L21.0213 10.3132C21.6447 10.8816 22 11.6863 22 12.5301V15C22 16.6569 20.6569 18 19 18H18V16H19C19.5523 16 20 15.5523 20 15V12.5301C20 12.2488 19.8816 11.9806 19.6738 11.7911L16.8989 9.26104C16.7147 9.0931 16.4744 9 16.2251 9H13V16H16V18H11V7Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M8 17C8 17.5523 7.55228 18 7 18C6.44772 18 6 17.5523 6 17C6 16.4477 6.44772 16 7 16C7.55228 16 8 16.4477 8 17ZM10 17C10 18.6569 8.65685 20 7 20C5.34315 20 4 18.6569 4 17C4 15.3431 5.34315 14 7 14C8.65685 14 10 15.3431 10 17ZM18 17C18 17.5523 17.5523 18 17 18C16.4477 18 16 17.5523 16 17C16 16.4477 16.4477 16 17 16C17.5523 16 18 16.4477 18 17ZM20 17C20 18.6569 18.6569 20 17 20C15.3431 20 14 18.6569 14 17C14 15.3431 15.3431 14 17 14C18.6569 14 20 15.3431 20 17Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconDeliveryTruck
