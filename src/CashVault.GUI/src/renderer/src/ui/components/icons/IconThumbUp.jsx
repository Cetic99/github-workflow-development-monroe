/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconThumbUp = ({ color = 'black', size = 's' }) => {
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
          d="M3 13C3 11.3431 4.34315 10 6 10H9V21H6C4.34315 21 3 19.6569 3 18V13ZM6 12C5.44772 12 5 12.4477 5 13V18C5 18.5523 5.44772 19 6 19H7V12H6Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M10.8483 4.22666C11.5691 3.00931 13.1555 2.63209 14.3471 3.39471L14.6757 3.60504C15.5434 4.16032 15.9816 5.19092 15.7796 6.20101L15.2198 8.99983H18.0007C19.8384 8.99983 21.2441 10.637 20.9662 12.4535L20.0482 18.4535C19.8241 19.9182 18.5644 20.9998 17.0827 20.9998H7V11.5475C7 11.0097 7.14457 10.4818 7.41857 10.0191L10.8483 4.22666ZM13.269 5.07925C13.0307 4.92673 12.7134 5.00217 12.5692 5.24564L9.13952 11.038C9.04819 11.1923 9 11.3683 9 11.5475V18.9998H17.0827C17.5766 18.9998 17.9965 18.6393 18.0712 18.1511L18.9892 12.1511C19.0819 11.5456 18.6133 10.9998 18.0007 10.9998H14.6099C13.6633 10.9998 12.9534 10.1338 13.139 9.20565L13.8184 5.80878C13.8588 5.60676 13.7712 5.40064 13.5976 5.28958L13.269 5.07925Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconThumbUp
